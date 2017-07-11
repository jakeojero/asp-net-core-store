$(function () {
    if ($("#register_popup") != undefined) {
        $('#register_popup').modal('show');
    }

    $('.nav-tabs a').on('show.bs.tab', function (e) {

        if ($(e.relatedTarget).text() === 'Personal') { // tab 1
            $('#Firstname').valid()
            $('#Lastname').valid()
            $('#Age').valid()
            $('#CreditcardType').valid()
            if ($('#Firstname').valid() === false || $('#Lastname').valid() === false || $('#CreditcardType').valid() === false || $('#Age').valid() === false) {
                return false; // suppress click
            }
        }
        if ($(e.relatedTarget).text() === 'Address') { // tab 2
            $('#Address1').valid()
            $('#City').valid()
            $('#Region').valid()
            $('#Country').valid()
            $('#Mailcode').valid()
            if ($('#Address1').valid() === false || $('#City').valid() === false || $('#Region').valid() === false || $('#Country').valid() === false || $('#Mailcode').valid() === false) {
                return false; // suppress click
            }
        }
        if ($(e.relatedTarget).text() === 'Account') { // tab 3
            $('#Email').valid()
            $('#Password').valid()
            $('#RepeatPassword').valid()
            if ($('#Email').valid() === false || $('#Password').valid() === false || $('#RepeatPassword').valid() === false) {
                return false; // suppress click
            }
        }
    }); // show bootstrap tab
});