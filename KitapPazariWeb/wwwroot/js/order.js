var dataTable;

   

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#dataTable').DataTable({
        "ajax": { url: '/admin/order/getall' },
        "columns": [
            { data: 'id', "width": "5%" },
            { data: 'name', "width": "15%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'applicationUser.email', "width": "20%" },
            { data: 'orderStatus', "width": "15%" },
            { data: 'orderTotal', "width": "15%" },
            {
                data: 'id', "render": function (data) {
                    return ` <div class="w-100 btn-group " role="group">
                         <a href="/admin/order/details?orderId=${data}" class="btn btn-primary  " >  <i class="bi bi-pencil-square"></i> Update </a>
                        
                    </div> `
                }, "width": "30%"
            }
        ]
    });
}
