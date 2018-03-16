
$(document).ready(function () {
    $("#encryptBtn").click(function () {
        var inputString = $("#rawDataTxt").val();
        var encodedInput = encodeURIComponent(inputString);

        $.get("api/data/EncryptString?val=" + encodedInput, function (resp, status) {
            if (resp.success == true) {
                $("#encryptedDataTxt").val(resp.data[0]);
                $("#keyDataTxt").val(resp.data[1]);

                $("#computedUrlTxt").val("/api/data/DecryptString?val=" + resp.data[0] + "&key=" + resp.data[1]);
            }
            else {
                alert("Error: " + resp.message);
            }
        });
    });

    $("#decryptBtn").click(function () {

        $.get($("#computedUrlTxt").val(), function (resp, status) {
            if (resp.success == true) {
                $("#decryptDataTxt").val(resp.data);
            }
            else {
                alert("Error: " + resp.message);
            }
        });
    });
});