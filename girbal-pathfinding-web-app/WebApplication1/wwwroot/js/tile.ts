class Tile {
    public x: number;
    public y: number;
    public element: HTMLElement; //<td>
    public tileSize: number = 30;
    public history: Status[] = [Status.NONE];

    private _status: Status = Status.NONE;
    private _agentId: number; //null for NONE, OBSTACLE
    private _backgroundImage: SVGElement; //null for NONE, OBSTACLE
    //private startGoalColors: string[] = ["green", "blue", "orange", "black", "turquoise", "pink", "purple"]
    private startGoalColors: number[][] = [[55, 219, 110], [55, 142, 219], [0, 0, 0], [255, 165, 0], [64, 224, 208], [255, 192, 203], [128, 0, 128]]
    private pathIsDrawn: boolean;

    public constructor(x: number, y: number, element: HTMLElement) {
        this.x = x;
        this.y = y;
        this.element = element;

        //this.element.setAttribute("width", this.tileSize.toString());
        //this.element.setAttribute("height", this.tileSize.toString());
        //console.log(this.element.offsetWidth)
    }

    public get status(): Status {
        return this._status;
    }

    public get agentId(): number {
        return this._agentId;
    }

    public set agentId(agentId: number) {
        this._agentId = agentId;
    }

    public set status(status: Status) {
        if (status !== this._status) {
            this.history.push(status);
        }

        this._status = status;

        if (this._status === Status.OBSTACLE || this._status === Status.NONE) {
            this.element.style.backgroundColor = this._status.color;
            this.element.style.backgroundImage = ""
        } else {
            if (this._status !== Status.PATH) {
                this.element.style.backgroundColor = "";

                let bgImage = this._status.image.cloneNode(true) as Element;
                let color = this.startGoalColors[this.agentId - 1];
                bgImage.setAttribute("fill", `rgb(${color[0]},${color[1]},${color[2]})`);

                var encoded = window.btoa(bgImage.outerHTML);
                this.element.style.backgroundImage = `url(data:image/svg+xml;base64,${encoded})`;
                this.element.style.backgroundSize = "cover";
            } else {
                //console.log("setting path")
                this.element.style.backgroundColor = `rgba(${this.startGoalColors[this.agentId - 1]}, 0.31)`;
                this.element.style.backgroundImage = "";

            }
        }

        this.element.setAttribute("class", this._status.name)

    }

    public drawBackground() {
        this.element.style.backgroundColor = `rgba(${this.startGoalColors[this.agentId - 1]}, 0.31)`
    }

    public clearBackgroundColor() {
        this.element.style.backgroundColor = "transparent"
    }

    public clearBackground() {
        this.element.style.backgroundColor = "transparent";
        this.element.style.backgroundImage = "";
    }


}

class Status {
    static readonly NONE = new Status(0, "none", "white");
    static readonly OBSTACLE = new Status(-1, "obstacle", "red");
    static readonly PATH = new Status(1, "path", "blue", document.getElementById("points-template"));
    static readonly START = new Status(2, "start", "green", document.getElementById("arrow-template"));
    static readonly GOAL = new Status(3, "goal", "red", document.getElementById("target-template"));

    // private to disallow creating other instances of this type
    private constructor(public readonly value: number, public readonly name: string, public color: string, public readonly image?: Element) {

    }

    toString() {
        return this.value;
    }
}
