$(function () {
    // display message if modal still loaded i
    if ($('#detailsId').val() > 0 && $('#detailsId').val() != "") {
        var data = $('#modalbtn' + $('#detailsId').val()).data('details');
        CopyToModal($('#detailsId').val(), data);
        $('#details_popup').modal('show');
    } //details
    // details anchor click - to load popup on catalogue
    $("a.btn-primary").on("click", function (e) {
        var Id = $(this).data("id");
        var data = $(this).data('details');
        $("#results").text("");
        CopyToModal(Id, data);
    });

    $("#btnRemove").on("click", function (e) {
        $("#qty").val(0);
    });
});

$(document).on('submit', '#brandsForm', function () {
    var $theForm = $(this);
    // manually trigger validation
    $.post('/Brand/SelectItem', $theForm.serialize())
        .done(function (response) {
            $('#results').text(response);
        })
    return false;
});

function CopyToModal(id, data) {
    $("#qty").val("0");
    $("#productPrice").text(data.CostPrice.toFixed(2));
    $("#productName").text(data.ProductName);

    $("#productDescription").text(data.Description);
    $("#productImage").attr("src", "/img/" + data.GraphicName + ".jpg");
    $("#detailsId").val(id);
}
