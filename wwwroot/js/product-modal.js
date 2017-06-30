$(function () {
    // display message if modal still loaded i
    if ($('#detailsId').val() > 0) {
        var data = $('#modalbtn' + $('#detailsId').val()).data('details');
        CopyToModal($('#detailsId').val(), data);
        $('#details_popup').modal('show');
    } //details
    // details anchor click - to load popup on catalogue
    $("a.btn-default").on("click", function (e) {
        var Id = $(this).data("id");
        var data = $(this).data('details');
        $("#results").text("");
        CopyToModal(Id, data);
    });
});
function CopyToModal(id, data) {
    $("#qty").val("0");
    $("#productPrice").text(data.CostPrice);
    $("#productName").text(data.ProductName);

    $("#productDescription").text(data.Description);
    $("#productImage").attr("src", "/img/" + data.GraphicName + ".jpg");
    $("#detailsId").val(id);
}