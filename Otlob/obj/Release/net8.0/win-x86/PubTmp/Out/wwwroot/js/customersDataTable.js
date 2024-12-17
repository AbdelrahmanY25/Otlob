$(document).ready(function () {
    $('#customers').dataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": "/api/customersApi",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{
            "targets": [0],
            "visible": false,
            "searchable": false
        }],
        "columns": [
            { "data": "id", "name": "Id", "autowidth": true },
            { "data": "userName", "name": "UserName", "autowidth": true },
            { "data": "phoneNumber", "name": "PhoneNumber", "autowidth": true },
            { "data": "email", "name": "Email", "autowidth": true },
        ]
    });
});
