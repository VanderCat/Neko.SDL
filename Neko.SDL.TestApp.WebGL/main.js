function mainLoop() {
    if (globalThis.myExports.Neko.SDL.TestApp.WebGL.Program.Frame())
        window.requestAnimationFrame(mainLoop);
    else
        globalThis.myExports.Neko.SDL.TestApp.WebGL.Program.Shutdown();
}


function createCanvas() {
    let canvas = document.createElement("canvas");
    canvas.width = 1280
    canvas.height = 720
    canvas.id = "canvas"
    Module.canvas = document.getElementsByClassName("uno-loader")[0].replaceWith(canvas)
}
async function init(){
    globalThis.myExports = await Module.getAssemblyExports("Neko.SDL.TestApp.WebGL");
    window.requestAnimationFrame(mainLoop);
}