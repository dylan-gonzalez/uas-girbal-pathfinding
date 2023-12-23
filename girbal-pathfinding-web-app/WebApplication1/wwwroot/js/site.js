mapWidth = 50;
mapHeight = 22;
noOfAgents = 1;

console.log("Height: ", mapHeight);

core = new Core(
    mapWidth,
    mapHeight
);

var current = new PathfindingRequestBody();


document.getElementById("agents-quantity").innerHTML = noOfAgents;
function adjustNoOfAgents(qty) {
    if (qty == 1) {
        noOfAgents += 1;
    } else {
        if (noOfAgents > 1) {
            noOfAgents -= 1;
        }
    }
    core.noOfAgents = noOfAgents;

    //document.getElementById("agents-quantity").innerHTML = noOfAgents;
    document.getElementById("agents-quantity").innerHTML = noOfAgents;
}

async function moveAlongPath(value) {
    let timestep = await core.moveAlongPath(value);
    console.log(timestep);
    document.getElementById("moveAlongPathLabel").innerHTML = timestep;
}

$(window).resize(function () {
    core.setTileSize();
})

function start() {
    if (core.createPathfindingRequestBody(current) == PathfindingRequestStatus.Ready) {
        console.log(current);
        $.ajax({
            type: "POST",
            url: "/pathfinding",
            data: JSON.stringify(current),
            contentType: "application/json",
            success: function (response) {
                var solution = response.data.solution;

                //solution = solution.map(agent => { return { id: agent.id, path: agent.path } });

                console.log(solution);

                core.drawSolution(solution)
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var msg = "";
                switch (jqXHR.status) {
                    case 400:
                        msg = "// The selected algorithm needs at least one Heuristic function.";
                        $(':input[name="heuristic"]').parent().css("color", "red");
                        break;
                    case 500:
                        msg = "// Something went wrong. Please try again later or report an issue at GitHub.";
                        break;
                }
                $("#exampleSelectMany").find("code").text(msg);
                $("#exampleExcept").find("code").text(msg);
                $("#exampleWhere").find("code").text(msg);
            },
            complete: function () {
                // TODO:
            }
        });
    }


}