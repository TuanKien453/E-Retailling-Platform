$(document).ready(function () {
    $.ajax({
        url: 'address/getProvince',
        type: 'GET',

        success: function (data_tinh) {
            console.log(data_tinh);
            if (data_tinh.code === 200) { 
                $.each(data_tinh.data, function (key_tinh, val_tinh) {
                    $("#tinh").append('<option value="' + val_tinh.ProvinceID + '">' + val_tinh.ProvinceName + '</option>');
                });

                $("#tinh").change(function () {
                    var idtinh = $(this).val();
                    $("#quan").html('<option value="0">Quận Huyện</option>');
                    $("#phuong").html('<option value="0">Phường Xã</option>');
                    $('#address').val(""); 

                    $.ajax({
                        url: 'https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/district?province_id=' + idtinh,
                        type: 'GET',
                        headers: {
                            'Token': '38a65a5a-90f7-11ef-8e53-0a00184fe694'
                        },
                        success: function (data_quan) {
                            if (data_quan.code === 200) {
                                $.each(data_quan.data, function (key_quan, val_quan) {
                                    $("#quan").append('<option value="' + val_quan.DistrictID + '">' + val_quan.DistrictName + '</option>');
                                });
                            }
                        }
                    });
                });

                $("#quan").change(function () {
                    var idquan = $(this).val();
                    var selectedProvince = $("#tinh option:selected").text();
                    $("#phuong").html('<option value="0">Phường Xã</option>');
                    $('#address').val(selectedProvince); 

                    $.ajax({
                        url: 'https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/ward?district_id=' + idquan,
                        type: 'GET',
                        headers: {
                            'Token': '38a65a5a-90f7-11ef-8e53-0a00184fe694'
                        },
                        success: function (data_phuong) {
                            if (data_phuong.code === 200) {
                                $.each(data_phuong.data, function (key_phuong, val_phuong) {
                                    $("#phuong").append('<option value="' + val_phuong.WardCode + '">' + val_phuong.WardName + '</option>');
                                });
                            }
                        }
                    });
                });

                $("#phuong").change(function () {
                    var selectedCommune = $(this).find("option:selected").text();
                    var selectedDistrict = $("#quan option:selected").text();
                    var selectedProvince = $("#tinh option:selected").text();
                    $('#address').val(selectedProvince + ', ' + selectedDistrict + ', ' + selectedCommune);
                });
            } else {
                console.error("Lỗi khi lấy tỉnh thành:", data_tinh.message);
            }
        },
        error: function (xhr, status, error) {
            console.error("Có lỗi xảy ra:", error);
            console.log("Mã trạng thái:", xhr.status); 
        }
    });
});