using Newtonsoft.Json;
using System.Collections.Generic;

namespace E_Retalling_Portal.Models.GHNResponseModel
{
    public class OrderInfoResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public OrderInfoData Data { get; set; }
    }

    public class OrderInfoData
    {
        [JsonProperty("shop_id")]
        public int? ShopId { get; set; }

        [JsonProperty("client_id")]
        public int? ClientId { get; set; }

        [JsonProperty("return_name")]
        public string? ReturnName { get; set; }

        [JsonProperty("return_phone")]
        public string? ReturnPhone { get; set; }

        [JsonProperty("return_address")]
        public string? ReturnAddress { get; set; }

        [JsonProperty("return_ward_code")]
        public string? ReturnWardCode { get; set; }

        [JsonProperty("return_district_id")]
        public int? ReturnDistrictId { get; set; }

        [JsonProperty("from_name")]
        public string? FromName { get; set; }

        [JsonProperty("from_phone")]
        public string? FromPhone { get; set; }

        [JsonProperty("from_address")]
        public string? FromAddress { get; set; }

        [JsonProperty("from_ward_code")]
        public string? FromWardCode { get; set; }

        [JsonProperty("from_district_id")]
        public int? FromDistrictId { get; set; }

        [JsonProperty("deliver_station_id")]
        public int? DeliverStationId { get; set; }

        [JsonProperty("to_name")]
        public string? ToName { get; set; }

        [JsonProperty("to_phone")]
        public string? ToPhone { get; set; }

        [JsonProperty("to_address")]
        public string? ToAddress { get; set; }

        [JsonProperty("to_ward_code")]
        public string? ToWardCode { get; set; }

        [JsonProperty("to_district_id")]
        public int? ToDistrictId { get; set; }

        [JsonProperty("weight")]
        public int? Weight { get; set; }

        [JsonProperty("length")]
        public int? Length { get; set; }

        [JsonProperty("width")]
        public int? Width { get; set; }

        [JsonProperty("height")]
        public int? Height { get; set; }

        [JsonProperty("converted_weight")]
        public int? ConvertedWeight { get; set; }

        [JsonProperty("service_type_id")]
        public int? ServiceTypeId { get; set; }

        [JsonProperty("service_id")]
        public int? ServiceId { get; set; }

        [JsonProperty("payment_type_id")]
        public int? PaymentTypeId { get; set; }

        [JsonProperty("cod_amount")]
        public int? CodAmount { get; set; }

        [JsonProperty("cod_collect_date")]
        public string? CodCollectDate { get; set; }

        [JsonProperty("cod_transfer_date")]
        public string? CodTransferDate { get; set; }

        [JsonProperty("insurance_value")]
        public bool? InsuranceValue { get; set; }

        [JsonProperty("pick_station_id")]
        public bool? PickStationId { get; set; }

        [JsonProperty("client_order_code")]
        public int? ClientOrderCode { get; set; }

        [JsonProperty("required_note")]
        public string? RequiredNote { get; set; }

        [JsonProperty("content")]
        public string? Content { get; set; }

        [JsonProperty("pickup_time")]
        public string? PickupTime { get; set; }

        [JsonProperty("note")]
        public string? Note { get; set; }

        [JsonProperty("employee_note")]
        public string? EmployeeNote { get; set; }

        [JsonProperty("coupon")]
        public string? Coupon { get; set; }

        [JsonProperty("order_code")]
        public string? OrderCode { get; set; }

        [JsonProperty("updated_ip")]
        public string? UpdatedIp { get; set; }

        [JsonProperty("updated_employee")]
        public int? UpdatedEmployee { get; set; }

        [JsonProperty("updated_client")]
        public int? UpdatedClient { get; set; }

        [JsonProperty("updated_source")]
        public string? UpdatedSource { get; set; }

        [JsonProperty("updated_date")]
        public string? UpdatedDate { get; set; }

        [JsonProperty("updated_warehouse")]
        public int? UpdatedWarehouse { get; set; }

        [JsonProperty("created_ip")]
        public string? CreatedIp { get; set; }

        [JsonProperty("created_employee")]
        public int? CreatedEmployee { get; set; }

        [JsonProperty("created_client")]
        public int? CreatedClient { get; set; }

        [JsonProperty("created_source")]
        public string? CreatedSource { get; set; }

        [JsonProperty("created_date")]
        public string? CreatedDate { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("pick_warehouse_id")]
        public int? PickWarehouseId { get; set; }

        [JsonProperty("deliver_warehouse_id")]
        public int? DeliverWarehouseId { get; set; }

        [JsonProperty("current_warehouse_id")]
        public int? CurrentWarehouseId { get; set; }

        [JsonProperty("return_warehouse_id")]
        public int? ReturnWarehouseId { get; set; }

        [JsonProperty("next_warehouse_id")]
        public int? NextWarehouseId { get; set; }

        [JsonProperty("leadtime")]
        public string? Leadtime { get; set; }

        [JsonProperty("order_date")]
        public string? OrderDate { get; set; }

        [JsonProperty("finish_date")]
        public string? FinishDate { get; set; }
    }



}
