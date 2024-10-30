$(document).ready(function () {
    // Lấy danh sách tỉnh thành từ backend
    $.ajax({
        url: '/address/getProvinces',
        type: 'GET',
        success: function (data_tinh) {
            console.log(data_tinh);
            $.each(data_tinh, function (key_tinh, val_tinh) {
                $("#tinh").append('<option value="' + val_tinh.provinceID + '">' + val_tinh.provinceName + '</option>');
            });

            $("#tinh").change(function () {
                var idtinh = $(this).val();
                $("#quan").html('<option value="0">Quận Huyện</option>');
                $("#phuong").html('<option value="0">Phường Xã</option>');
                $('#address').val("");

                $.ajax({
                    url: '/address/getDistricts?provinceID=' + idtinh,
                    type: 'GET',
                    contentType: 'application/json',
                    data: JSON.stringify(idtinh), // Gửi id tỉnh dưới dạng JSON
                    success: function (data_quan) {
                        $.each(data_quan, function (key_quan, val_quan) {
                            $("#quan").append('<option value="' + val_quan.districtID + '">' + val_quan.districtName + '</option>');
                        });
                    },
                    error: function (xhr, status, error) {
                        console.error("Có lỗi khi lấy quận huyện:", error);
                    }
                });
            });

            $("#quan").change(function () {
                var idquan = $(this).val();
                var selectedProvince = $("#tinh option:selected").text();
                $("#phuong").html('<option value="0">Phường Xã</option>');
                $('#address').val(selectedProvince);

                $.ajax({
                    url: '/address/getWards?districtId=' + idquan,
                    type: 'GET',
                    contentType: 'application/json',
                    data: JSON.stringify(idquan),
                    success: function (data_phuong) {
                        $.each(data_phuong, function (key_phuong, val_phuong) {
                            $("#phuong").append('<option value="' + val_phuong.wardCode + '">' + val_phuong.wardName + '</option>');
                        });
                    },
                    error: function (xhr, status, error) {
                        console.error("Có lỗi khi lấy phường xã:", error);
                    }
                });
            });

            $("#phuong").change(function () {
                var selectedCommune = $(this).find("option:selected").text();
                var selectedDistrict = $("#quan option:selected").text();
                var selectedProvince = $("#tinh option:selected").text();
                $('#address').val(selectedProvince + ', ' + selectedDistrict + ', ' + selectedCommune);
            });
        },
        error: function (xhr, status, error) {
            console.error("Có lỗi xảy ra:", error);
            console.log("Mã trạng thái:", xhr.status);
        }
    });
});