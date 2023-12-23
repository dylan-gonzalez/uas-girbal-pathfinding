/*
const grid = document.getElementById("grid");

const gridLeft = grid.getBoundingClientRect().left;
const gridTop = grid.getBoundingClientRect().top;

const mapWidth = 25;
const mapHeight = 20;

let dragging = false;

let obstacles = [];

let currentAgent = 1;
let agents = [];


let state = "obstacle";

for (let i = 0; i < mapWidth; i++) {
    let newCol = document.createElement("ul");

    for (let j = 0; j < mapHeight; j++) {
        //grid.innerHTML += "<li class='grid__tile'></li>"
        let tile = document.createElement("li");
        tile.className = "grid__tile";
        newCol.appendChild(tile);
    }

    grid.appendChild(newCol);
}

//adding start/goal states manually for now
let startStates = [{ x: 4, y: 13 }]
let goalStates = [{ x: 18, y: 3 }]

for (let i = 0; i < startStates.length; i++) {
    $(grid.children[startStates[i].x].children[startStates[i].y]).css("background-color", "green");
}

for (let i = 0; i < goalStates.length; i++) {
    $(grid.children[goalStates[i].x].children[goalStates[i].y]).css("background-color", "red");
}


$('#grid').mouseover(function () {
    $(this).mousedown(function () {
        console.log("down + over")
        dragging = true;

    })


})

$("#grid").mouseup(function () {
    dragging = false;
    console.log("up")
});


$(".grid__tile").mouseover(function (event, element) {
    let rect = event.target.getBoundingClientRect();
    var y = event.clientY;
    var x = event.clientX;

    if ((y >= rect.top || y < rect.bottom) && (x >= rect.left || x < rect.right)) {
        var xIndex = (rect.left - gridLeft) / rect.width;
        var yIndex = (rect.top - gridTop) / rect.height;

        let item = { x: xIndex, y: yIndex }

        let prevState = null;


        let oldTileColour = $(grid.children[xIndex].children[yIndex]).css("background-color");
        //console.log("recent colour: ", tileEntered.css("background-color"))

        switch (state) {
            case "obstacle":
                if (dragging) {
                    $(this).css("background", "gray")
                    //add obstacle to staticObstacles


                    if (!obstacles.includes(item)) {
                        obstacles.push(item);
                    }
                }
                break;

            //case "start":
            //    let startStateSelected = false;

            //    $(this).css("background", "green");

            //    $(this).mouseleave(function () {
            //        if (!startStateSelected)
            //            $(this).css("background-color", oldTileColour);
            //        //$(this).css("background", "red");
            //    })

            //    $(this).mousedown(function () {
                    
            //        prevState = agents[currentAgent - 1].startState;
            //        console.log("prev: ", prevState);
            //        //add start state
            //        if (prevState == null) {
            //            prevState = item;
            //            agents[currentAgent - 1].startState = prevState;
            //            startStateSelected = true;
            //        } else {
            //            console.log(prevState);
            //            prevState = agents[currentAgent - 1].startState;


            //            //reset old start state colour
            //            $(grid.children[prevState.x].children[prevState.y]).css("background", "white");

            //            agents[currentAgent - 1].startState = item;
            //            startStateSelected = true;
            //        }

            //    })
            //    break;

            //case "goal":
            //    let goalStateSelected = false;
            //    $(this).css("background", "red");

            //    $(this).mouseleave(function () {
            //        if (!goalStateSelected)
            //            $(this).css("background-color", oldTileColour);
            //        //$(this).css("background", "red");
            //    })

            //    $(this).mousedown(function () {
            //        //add start state
            //        if (agents[currentAgent - 1] == undefined) {
            //            agents.push({ goalState: item });
            //            goalStateSelected = true;
                        
            //        } else {
            //            //reset goal state colour
            //            prevState = $(grid.children[agents[currentAgent - 1].goalState.x].children[agents[currentAgent - 1].goalState.y])

            //            if (prevState != undefined)
            //                prevState.css("background", "white");

            //            agents.goalState = item;
            //            goalStateSelected = true;
            //        }

            //    })
            //    break;

            default:
                console.log("none")
                break;


        }
        
    }
})


$(this).keypress(function (event) {
    var keycode = (event.keyCode ? event.keyCode : event.which);
    console.log("test: ", keycode);
    if (keycode == '49') { //1 --> select agent #1
        state = "start";
        agents.push({ startState: null, goalState: null})
    }

    if (keycode == '115') { //s --> select start state
        state = "start";
    }

    if (keycode == '103') {//g --> goal state
        state = "goal";
    }

    if (keycode == "111") { //o
        state = "obstacle"
    }
 });


$("#start-btn").click(function () {
    console.log("clicked")

    $.ajax({
        type: "POST",
        //url: "api/Pathfinding/Find",
        url: "/pathfinding",
        data: " ",
        contentType: "application/json",
        success: function (response) {
            console.log(response);
        },
        error: function (xhr) {
            console.log(xhr);
            alert('Error: ' + xhr.statusText);
        },
    })
})





*/

