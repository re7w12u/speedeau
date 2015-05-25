

    RNVO.obs = function () { }

    RNVO.obs.getObsByFileName = function (filename) {
        var name = filename.substring(0, filename.lastIndexOf('.'))

        // TODO : construct endpoint dynamically
        var requestUri = "http://rdits-sp13-dev/sites/rnvo/_vti_bin/listdata.svc/" + IH1600.param.depObsListName +
            "?$select=Title,Validation,Observation&$expand=Document&$filter=Document/Title%20eq%20'" + name + "'";

        $.ajax({
            url: requestUri,
            type: "GET",
            headers: { "ACCEPT": "application/json;odata=verbose" },
            success: function (data) {
                alert(data.d.FirstName);
            },
            error: function () {
                alert("Failed to get customer");
            }
        });

}


    RNVO.obs.registerClass("RNVO.obs");


//# sourceURL=rnvo.obs.js