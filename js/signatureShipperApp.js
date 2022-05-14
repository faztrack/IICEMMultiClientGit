var wrapper1 = document.getElementById("SignaturePadCustome"),
    clearButtonShipper = wrapper1.querySelector("[data-action=clear1]"),
    saveButtonShipper = wrapper1.querySelector("[data-action=save1]"),
    canvas = wrapper1.querySelector("canvas"),
    signaturePadShipper;


// Adjust canvas coordinate space taking into account pixel ratio,
// to make it look crisp on mobile devices.
// This also causes canvas to be cleared.
function resizeCanvas() {
    // When zoomed out to less than 100%, for some very strange reason,
    // some browsers report devicePixelRatio as less than 1
    // and only part of the canvas is cleared then.
    var ratio = Math.max(window.devicePixelRatio || 1, 1);
    canvas.width = canvas.offsetWidth * ratio;
    canvas.height = canvas.offsetHeight * ratio;
    var context = canvas.getContext("2d").scale(ratio, ratio);





}

window.onresize = resizeCanvas;
resizeCanvas();

signaturePadShipper = new SignaturePad(canvas);

clearButtonShipper.addEventListener("click", function (event) {
    event.preventDefault();
    //var answer = confirm("Are you sure to clear signature?")
    //if (answer)
    //signaturePadShipper.clear();

    signaturePadShipper.clear();
});

saveButtonShipper.addEventListener("click", function (event) {

    if (signaturePadShipper.isEmpty()) {
        alert("Please provide signature first.");
    } else {
        $('#hdnSignCustomer').val(signaturePadShipper.toDataURL());


    }


});