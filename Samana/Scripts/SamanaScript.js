function setMentorListbox() {

    if ($("#LidsoortId").val() == 0 || $("#LidsoortId").val() == 1) {
        $("#mentorId").val(0);
        $("#mentorId").prop('disabled', true);
    }
    else {
        $("#mentorId").prop('disabled', false);
    }
}
function adjust_textarea(h) {
    h.style.height = "20px";
    h.style.height = (h.scrollHeight) + "px";
}
