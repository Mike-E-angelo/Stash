var dotnetref;
function resized(instance) {
    dotnetref = instance;
    Checksize();
    window.onresize = function (e) {
        Checksize();
    };
}
function Checksize() {
	const rect = document.getElementsByClassName("required-container")[0].getBoundingClientRect();
	const width = rect.width;
    const adaptive = width < 500;
    dotnetref.invokeMethodAsync('Resized', adaptive ? "Mobile" : "Desktop");
}