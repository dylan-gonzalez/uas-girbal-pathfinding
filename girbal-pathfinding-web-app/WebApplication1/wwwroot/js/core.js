var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class Core {
    constructor(mapWidth, mapHeight) {
        this.tileWidth = 31.13; //31.13;
        this._noOfAgents = 1;
        this.agents = [];
        this.solutionFound = false;
        this.delay = 50;
        this.overlappingStates = [];
        this.timer = ms => new Promise(res => setTimeout(res, ms));
        this.map = document.getElementById("map");
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        this.grid = new Array(mapHeight);
        console.log(this.mapWidth);
        //create grid
        for (let y = 0; y < mapHeight + 1; y++) {
            this.grid[y] = new Array(mapWidth);
            let newRow = document.createElement("tr");
            newRow.setAttribute("id", `row ${y}`);
            this.map.appendChild(newRow);
            for (let x = 0; x < mapWidth + 1; x++) {
                let newTileId = `${y}-${x}`, newTileClass, newTile, newTileElement;
                newTileClass = "none";
                newTileElement = document.createElement("td");
                //newTileElement.setAttribute("width", this.tileSize.toString() + "px")
                newTileElement.setAttribute("id", newTileId);
                newTileElement.setAttribute("class", newTileClass);
                newRow.appendChild(newTileElement);
                newTile = new Tile(x, y, newTileElement);
                this.grid[y][x] = newTile;
            }
        }
        this.setTileSize();
        //initial start/goal tiles
        let startCoords = { x: Math.floor(mapWidth / 2) - 5, y: Math.floor(mapHeight / 2), time: 0 };
        let goalCoords = { x: Math.floor(mapWidth / 2) + 5, y: Math.floor(mapHeight / 2), time: 0 };
        let agent = { startState: startCoords, goalState: goalCoords, id: 1 };
        this.agents.push(agent);
        let startTile = this.grid[startCoords.y][startCoords.x];
        let goalTile = this.grid[goalCoords.y][goalCoords.x];
        startTile.agentId = 1;
        goalTile.agentId = 1;
        startTile.status = Status.START;
        goalTile.status = Status.GOAL;
        //add event listeners
        this.map.addEventListener("mousedown", (e) => this.onMouseDown(e));
        this.map.addEventListener("mousemove", (e) => this.onMouseMove(e));
        this.map.addEventListener("mouseup", (e) => this.onMouseUp(e));
    }
    get noOfAgents() {
        return this._noOfAgents;
    }
    set noOfAgents(value) {
        let change = value - this._noOfAgents;
        if (change > 0) {
            let agent = { startState: { x: null, y: null, time: 0 }, goalState: { x: null, y: null, time: 0 }, id: value };
            let randomY, randomX, tile;
            for (let i = 0; i < 2; i++) {
                randomY = Math.floor(Math.random() * (this.grid.length - 1));
                randomX = Math.floor(Math.random() * (this.grid[0].length - 1)); //assuming this.grid is a square matrix
                tile = this.grid[randomY][randomX];
                while (tile.status !== Status.NONE) {
                    randomY = Math.floor(Math.random() * (this.grid.length - 1));
                    randomX = Math.floor(Math.random() * (this.grid[0].length - 1));
                    tile = this.grid[randomY][randomX];
                }
                tile.agentId = value;
                tile.status = i === 0 ? Status.START : Status.GOAL;
                if (i === 0) {
                    agent.startState = { x: randomX, y: randomY, time: 0 };
                }
                else {
                    agent.goalState = { x: randomX, y: randomY, time: 0 };
                }
            }
            this.agents.push(agent);
        }
        else if (change < 0) {
            //remove
            let tile;
            for (let j = 0; j < this.mapHeight; j++) {
                for (let i = 0; i < this.mapWidth; i++) {
                    tile = this.grid[j][i];
                    if (tile.agentId === this.noOfAgents) {
                        tile.status = Status.NONE;
                        tile.agentId = null;
                    }
                }
            }
            this.agents.pop();
        }
        this._noOfAgents = value;
    }
    setTileSize() {
        this.tileWidth = document.getElementsByTagName("td")[0].offsetWidth;
        this.tileHeight = document.getElementsByTagName("td")[0].offsetHeight;
    }
    getCoords(x, y) {
        let map = this.map.getBoundingClientRect();
        let mouseX = Math.floor((x - map.left) / this.tileWidth);
        let mouseY = Math.floor((y - map.top) / this.tileHeight);
        return { x: mouseX, y: mouseY };
    }
    onMouseDown(event) {
        this.mouseDown = true;
        let coords = this.getCoords(event.clientX, event.clientY);
        let tile = this.grid[coords.y][coords.x];
        let statusType;
        if (tile.status === Status.NONE) {
            statusType = Status.OBSTACLE;
        }
        else if (tile.status === Status.OBSTACLE) {
            statusType = Status.NONE;
        }
        else if (tile.status === Status.START) {
            statusType = Status.START;
        }
        else {
            statusType = Status.GOAL;
        }
        tile.status = statusType;
        this.settingTileType = statusType;
        this.currentTile = tile;
    }
    onMouseUp(event) {
        this.mouseDown = false;
    }
    onMouseMove(event) {
        let oldTile;
        let coords = this.getCoords(event.clientX, event.clientY);
        let newTile;
        try {
            newTile = this.grid[coords.y][coords.x];
        }
        catch (_a) {
        }
        if (this.currentTile !== newTile) {
            oldTile = this.currentTile;
            this.currentTile = newTile;
            if (this.mouseDown) {
                if (this.settingTileType === Status.START || this.settingTileType === Status.GOAL) {
                    newTile.agentId = oldTile.agentId;
                    newTile.status = this.settingTileType;
                    oldTile.agentId = null;
                    oldTile.status = oldTile.history[oldTile.history.length - 2];
                    if (this.settingTileType === Status.START) {
                        this.agents[newTile.agentId - 1].startState = { x: coords.x, y: coords.y, time: 0 };
                    }
                    else {
                        this.agents[newTile.agentId - 1].goalState = { x: coords.x, y: coords.y, time: 0 };
                    }
                }
                else {
                    //disallow overriding start/goal state with obstacle/none state
                    if (!(newTile.status === Status.START || newTile.status === Status.GOAL)) {
                        newTile.status = this.settingTileType;
                    }
                }
            }
        }
    }
    createPathfindingRequestBody(body) {
        this.clear();
        body.agents = this.agents;
        let grid_temp = new Array(this.mapHeight);
        for (let j = 0; j < this.mapHeight; j++) {
            grid_temp[j] = new Array(this.mapWidth);
            for (let i = 0; i < this.mapWidth; i++) {
                grid_temp[j][i] = this.grid[j][i].status.value;
            }
        }
        body.map = grid_temp;
        return PathfindingRequestStatus.Ready; // Ready for sending request.
    }
    drawSolution(agents) {
        return __awaiter(this, void 0, void 0, function* () {
            for (let a = 0; a < agents.length; a++) {
                agents[a].path.reverse();
                //add start state to the path
                agents[a].path.splice(0, 0, this.agents[agents[a].id - 1].startState);
                this.agents[a].path = agents[a].path;
            }
            yield this.drawPaths();
            this.solutionFound = true;
        });
    }
    drawPaths() {
        return __awaiter(this, void 0, void 0, function* () {
            this.clear();
            let maxPathLength = 0;
            for (let i = 0; i < this.agents.length; i++) {
                if (this.agents[i].path.length > maxPathLength) {
                    maxPathLength = this.agents[i].path.length;
                }
            }
            for (let j = 0; j < maxPathLength; j++) {
                for (let i = 0; i < this.agents.length; i++) {
                    let index = this.agents[i].path.findIndex(state => state === this.agents[i].startState);
                    if (j < index || j >= this.agents[i].path.length) {
                        continue;
                    }
                    let state = this.agents[i].path[j];
                    let tile = this.grid[state.y][state.x];
                    let overlappingAgent = tile.agentId !== null ? this.agents.find(agent => agent.id === tile.agentId && agent.id !== this.agents[i].id) : undefined;
                    let overlappingState = overlappingAgent !== undefined ? overlappingAgent.path.find(_state => _state.x === state.x && _state.y === state.y) : null;
                    if (overlappingState === null || overlappingState.time > state.time || this.agents[i].startState.time > overlappingState.time) {
                        tile.agentId = this.agents[i].id;
                        if (j === index) {
                            tile.status = Status.START;
                            tile.drawBackground();
                        }
                        else if (j < this.agents[i].path.length - 1) {
                            tile.status = Status.PATH;
                        }
                        else {
                            tile.status = Status.GOAL;
                            tile.drawBackground();
                        }
                    }
                }
                if (!this.solutionFound) {
                    yield this.timer(this.delay);
                }
            }
        });
    }
    //public async drawPaths() {
    //    this.clear();
    //    for (let i = 0; i < this.agents.length; i++) {
    //        let index = this.agents[i].path.findIndex(state => state === this.agents[i].startState);
    //        for (let j = index; j < this.agents[i].path.length; j++) {
    //            let state = this.agents[i].path[j];
    //            let tile = this.grid[state.y][state.x];
    //            let overlappingAgent = tile.agentId !== null ? this.agents.find(agent => agent.id === tile.agentId && agent.id !== this.agents[i].id) : undefined;
    //            let overlappingState = overlappingAgent !== undefined ? overlappingAgent.path.find(_state => _state.x === state.x && _state.y === state.y) : null;
    //            if (overlappingState === null || overlappingState.time > state.time || this.agents[i].startState.time > overlappingState.time) {
    //                tile.agentId = this.agents[i].id;
    //                if (j === index) {
    //                    tile.status = Status.START;
    //                    tile.drawBackground();
    //                } else if (j < this.agents[i].path.length - 1) {
    //                    tile.status = Status.PATH;
    //                } else {
    //                    tile.status = Status.GOAL;
    //                    tile.drawBackground();
    //                }
    //            }
    //            if (!this.solutionFound) {
    //                await this.timer(this.delay);
    //            }
    //        }
    //    }
    //}
    moveAlongPath(value) {
        let maxPathLength = 0;
        let agentWithLongestPath;
        //get longest path value
        for (let i = 0; i < this.agents.length; i++) {
            if (this.agents[i].path.length > maxPathLength) {
                maxPathLength = this.agents[i].path.length;
                agentWithLongestPath = this.agents[i];
            }
        }
        let timestepValue = 100 / maxPathLength; //calculate the value of each step in slider to move 1 timestep forward
        let index = Math.floor(value / timestepValue); //current path index
        for (let i = 0; i < this.agents.length; i++) {
            if (index < this.agents[i].path.length) {
                this.agents[i].startState = this.agents[i].path[index];
            }
            else {
                this.agents[i].startState = this.agents[i].path[this.agents[i].path.length - 1];
            }
        }
        this.drawPaths();
        return index;
    }
    /*
    public async moveAlongPath(value: number) {
        console.log("moveAlongPath")
        let maxPathLength = 0, agentWithLongestPath: Agent, timestepValue, index, prevIndex, lowerBound, upperBound, diff, currentTile, previousTile;

        //get longest path value
        for (let i = 0; i < this.agents.length; i++) {
            if (this.agents[i].path.length > maxPathLength) {
                maxPathLength = this.agents[i].path.length;
                agentWithLongestPath = this.agents[i];
            }
        }

        timestepValue = 100 / maxPathLength; //calculate the value of each step in slider to move 1 timestep forward
        index = Math.floor(value / timestepValue); //current path index
        prevIndex = agentWithLongestPath.path.indexOf(agentWithLongestPath.startState); //previous path index

        if (index >= prevIndex) {
            lowerBound = prevIndex;
            upperBound = index;
            diff = 1;
        } else {
            lowerBound = index;
            upperBound = prevIndex;
            diff = -1
        }

        for (let i = 0; i < this.agents.length; i++) {
            let path = [...this.agents[i].path];

            path = path.slice(lowerBound, upperBound+1); //path must include both prevIndex and index

            if (diff === -1) {
                //reverse the path to set the statuses of the path in the order the startState moves from
                path.reverse();
            }

            for (let j = 0; j < path.length; j++) {
                if (j + 1 >= path.length) {
                    //index out of range; skip to next agent
                    continue;
                }

                currentTile = this.grid[path[j + 1].y][path[j + 1].x];
                previousTile = this.grid[path[j].y][path[j].x];

                //reversing
                if (diff === - 1) {
                    if (previousTile !== null) {
                        previousTile.status = Status.PATH;
                    }
                    currentTile.status = Status.START;
                    currentTile.drawBackground();

                    this.grid[this.agents[i].goalState.y][this.agents[i].goalState.x].status = Status.GOAL;
                    this.grid[this.agents[i].goalState.y][this.agents[i].goalState.x].drawBackground();
                }
                //going forward
                else {
                    if (previousTile !== null) {
                        previousTile.status = Status.NONE;
                    }
                    currentTile.status = Status.START;
                    currentTile.drawBackground();
                }

                this.agents[i].startState = path[j + 1];

                //await this.timer(this.delay);

            }

        }

        this.drawPaths();
    }
    */
    clear() {
        //clear all existing paths
        console.log("clear");
        if (!this.solutionFound) {
            return;
        }
        for (let i = 0; i < this.agents.length; i++) {
            if (this.agents[i].path === undefined) {
                continue;
            }
            for (let j = 0; j < this.agents[i].path.length; j++) {
                let state = this.agents[i].path[j];
                let tile = this.grid[state.y][state.x];
                if (tile.status === Status.PATH) {
                    tile.status = Status.NONE;
                }
                else {
                    tile.clearBackground();
                }
            }
        }
    }
}
var PathfindingRequestStatus;
(function (PathfindingRequestStatus) {
    PathfindingRequestStatus[PathfindingRequestStatus["None"] = 0] = "None";
    PathfindingRequestStatus[PathfindingRequestStatus["Initiated"] = 1] = "Initiated";
    PathfindingRequestStatus[PathfindingRequestStatus["Ready"] = 2] = "Ready";
})(PathfindingRequestStatus || (PathfindingRequestStatus = {}));
class PathfindingRequestBody {
    constructor() {
        this.agents = [];
        this.map = [];
    }
}
