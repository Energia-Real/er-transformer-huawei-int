namespace er_transformer_huawei_int.Models
{
    using System;
    using System.Collections.Generic;

    public class DeviceInverterDataItem
    {
        public double pv2_u { get; set; }
        public double pv4_u { get; set; }
        public double pv22_i { get; set; }
        public double pv6_u { get; set; }
        public double power_factor { get; set; }
        public double mppt_total_cap { get; set; }
        public double pv24_i { get; set; }
        public double pv8_u { get; set; }
        public double open_time { get; set; }
        public double pv22_u { get; set; }
        public double a_i { get; set; }
        public double pv24_u { get; set; }
        public double c_i { get; set; }
        public double mppt_9_cap { get; set; }
        public double pv20_u { get; set; }
        public double pv19_u { get; set; }
        public double pv15_u { get; set; }
        public double pv17_u { get; set; }
        public double reactive_power { get; set; }
        public double a_u { get; set; }
        public double c_u { get; set; }
        public double mppt_8_cap { get; set; }
        public double pv20_i { get; set; }
        public double pv15_i { get; set; }
        public double efficiency { get; set; }
        public double pv17_i { get; set; }
        public double pv11_i { get; set; }
        public double pv13_i { get; set; }
        public double pv11_u { get; set; }
        public double pv13_u { get; set; }
        public double mppt_power { get; set; }
        public double close_time { get; set; }
        public double pv19_i { get; set; }
        public double mppt_7_cap { get; set; }
        public double mppt_5_cap { get; set; }
        public double pv2_i { get; set; }
        public double active_power { get; set; }
        public double pv4_i { get; set; }
        public double pv6_i { get; set; }
        public double pv8_i { get; set; }
        public double mppt_6_cap { get; set; }
        public double pv1_u { get; set; }
        public double pv3_u { get; set; }
        public double pv23_i { get; set; }
        public double pv5_u { get; set; }
        public double pv7_u { get; set; }
        public double pv23_u { get; set; }
        public double inverter_state { get; set; }
        public double pv9_u { get; set; }
        public double total_cap { get; set; }
        public double b_i { get; set; }
        public double mppt_3_cap { get; set; }
        public double pv21_u { get; set; }
        public double mppt_10_cap { get; set; }
        public double pv16_u { get; set; }
        public double pv18_u { get; set; }
        public double temperature { get; set; }
        public double b_u { get; set; }
        public double bc_u { get; set; }
        public double pv21_i { get; set; }
        public double elec_freq { get; set; }
        public double mppt_4_cap { get; set; }
        public double pv16_i { get; set; }
        public double pv18_i { get; set; }
        public double day_cap { get; set; }
        public double pv12_i { get; set; }
        public double pv14_i { get; set; }
        public double pv12_u { get; set; }
        public double pv14_u { get; set; }
        public double mppt_1_cap { get; set; }
        public double pv10_u { get; set; }
        public double pv1_i { get; set; }
        public double pv3_i { get; set; }
        public double mppt_2_cap { get; set; }
        public double pv5_i { get; set; }
        public double ab_u { get; set; }
        public double ca_u { get; set; }
        public double pv10_i { get; set; }
        public double pv7_i { get; set; }
        public double pv9_i { get; set; }
    }

    public class DeviceMetterDataItem
    {
        public double active_cap { get; set; }
        public double power_factor { get; set; }
        public double a_i { get; set; }
        public double c_i { get; set; }
        public double b_i { get; set; }
        public string reverse_reactive_valley { get; set; }
        public string positive_reactive_peak { get; set; }
        public string reverse_reactive_peak { get; set; }
        public string positive_active_peak { get; set; }
        public string reverse_active_peak { get; set; }
        public double a_u { get; set; }
        public double reactive_power { get; set; }
        public double total_apparent_power { get; set; }
        public double c_u { get; set; }
        public double bc_u { get; set; }
        public double b_u { get; set; }
        public string reverse_reactive_power { get; set; }
        public double reverse_active_cap { get; set; }
        public double active_power_b { get; set; }
        public double active_power_a { get; set; }
        public string positive_active_top { get; set; }
        public double reverse_reactive_cap { get; set; }
        public string positive_active_valley { get; set; }
        public string positive_active_power { get; set; }
        public string reverse_reactive_top { get; set; }
        public string reverse_active_top { get; set; }
        public string reverse_active_power { get; set; }
        public string reactive_power_a { get; set; }
        public string reactive_power_b { get; set; }
        public double forward_reactive_cap { get; set; }
        public string reverse_active_valley { get; set; }
        public string reactive_power_c { get; set; }
        public double active_power { get; set; }
        public double ca_u { get; set; }
        public double ab_u { get; set; }
        public string positive_reactive_power { get; set; }
        public double active_power_c { get; set; }
        public string grid_frequency { get; set; }
    }

    public class DeviceDataResponse<T>
    {
        public long devId { get; set; }
        public long collectTime { get; set; }
        public T dataItemMap { get; set; }
    }

    public class DeviceParams
    {
        public long currentTime { get; set; }
        public long collectTime { get; set; }
        public string devIds { get; set; }
        public int devTypeId { get; set; }
    }

    public class DeviceFiveMinutesResponse<T>
    {
        public List<DeviceDataResponse<T>> data { get; set; }
        public int failCode { get; set; }
        public object message { get; set; }
        public Params @params { get; set; }
        public bool success { get; set; }
    }
}
