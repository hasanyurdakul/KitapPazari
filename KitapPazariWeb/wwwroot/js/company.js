var dataTable;



$(document).ready(function () {
    loadDataTable();
});

function loadDataTable(){
    dataTable = $('#dataTable').DataTable({
        "ajax": { url: '/admin/company/getall' },
        "columns": [
            { data: 'name', "width": "20%" },
            { data: 'streetAddress', "width": "15%" },
            { data: 'city', "width": "15%" },
            { data: 'state', "width": "5%" },
            { data: 'phoneNumber', "width": "15%" },
            {
                data: 'id', "render": function (data) {
                    return ` <div class="w-100 btn-group " role="group">
                         <a href="/admin/company/upsert?id=${data}" class="btn btn-primary  " >  <i class="bi bi-pencil-square"></i> Edit </a>
                         <a onClick=Delete('/admin/company/delete/${data}')  class="btn btn-danger  " >  <i class="bi bi-trash-fill"></i> Delete </a>
                    </div> `
            }   , "width": "30%" }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }

            })
        }
    });
}