class Tile {
    constructor(x, y, element) {
        this.tileSize = 30;
        this.history = [Status.NONE];
        this._status = Status.NONE;
        //private startGoalColors: string[] = ["green", "blue", "orange", "black", "turquoise", "pink", "purple"]
        this.startGoalColors = [[55, 219, 110], [55, 142, 219], [0, 0, 0], [255, 165, 0], [64, 224, 208], [255, 192, 203], [128, 0, 128]];
        this.x = x;
        this.y = y;
        this.element = element;
        //this.element.setAttribute("width", this.tileSize.toString());
        //this.element.setAttribute("height", this.tileSize.toString());
        //console.log(this.element.offsetWidth)
    }
    get status() {
        return this._status;
    }
    get agentId() {
        return this._agentId;
    }
    set agentId(agentId) {
        this._agentId = agentId;
    }
    set status(status) {
        if (status !== this._status) {
            this.history.push(status);
        }
        this._status = status;
        if (this._status === Status.OBSTACLE || this._status === Status.NONE) {
            this.element.style.backgroundColor = this._status.color;
            this.element.style.backgroundImage = "";
        }
        else {
            if (this._status !== Status.PATH) {
                this.element.style.backgroundColor = "";
                let bgImage = this._status.image.cloneNode(true);
                let color = this.startGoalColors[this.agentId - 1];
                bgImage.setAttribute("fill", `rgb(${color[0]},${color[1]},${color[2]})`);
                var encoded = window.btoa(bgImage.outerHTML);
                this.element.style.backgroundImage = `url(data:image/svg+xml;base64,${encoded})`;
                this.element.style.backgroundSize = "cover";
            }
            else {
                //console.log("setting path")
                this.element.style.backgroundColor = `rgba(${this.startGoalColors[this.agentId - 1]}, 0.31)`;
                this.element.style.backgroundImage = "";
            }
        }
        this.element.setAttribute("class", this._status.name);
    }
    drawBackground() {
        this.element.style.backgroundColor = `rgba(${this.startGoalColors[this.agentId - 1]}, 0.31)`;
    }
    clearBackgroundColor() {
        this.element.style.backgroundColor = "transparent";
    }
    clearBackground() {
        this.element.style.backgroundColor = "transparent";
        this.element.style.backgroundImage = "";
    }
}
class Status {
    // private to disallow creating other instances of this type
    constructor(value, name, color, image) {
        this.value = value;
        this.name = name;
        this.color = color;
        this.image = image;
    }
    toString() {
        return this.value;
    }
}
Status.NONE = new Status(0, "none", "white");
Status.OBSTACLE = new Status(-1, "obstacle", "red");
Status.PATH = new Status(1, "path", "blue", document.getElementById("points-template"));
Status.START = new Status(2, "start", "green", document.getElementById("arrow-template"));
Status.GOAL = new Status(3, "goal", "red", document.getElementById("target-template"));
