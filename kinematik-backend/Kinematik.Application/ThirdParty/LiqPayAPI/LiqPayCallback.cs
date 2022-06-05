namespace Kinematik.Application.ThirdParty.LiqPayAPI
{
    public class LiqPayCallback
    {
        public int acq_id { get; set; }
        public string action { get; set; }
        public decimal agent_commision { get; set; }
        public decimal amount { get; set; }
        public decimal amount_bonus { get; set; }
        public decimal amount_credit { get; set; }
        public decimal amount_debit { get; set; }
        public string authcode_credit { get; set; }
        public string authcode_debit { get; set; }
        public string card_token { get; set; }
        public decimal commission_credit { get; set; }
        public decimal commission_debit { get; set; }
        public string completion_date { get; set; }
        public string create_date { get; set; }
        public string currency { get; set; }
        public string currency_credit { get; set; }
        public string currency_debit { get; set; }
        public string customer { get; set; }
        public string description { get; set; }
        public string end_date { get; set; }
        public string err_code { get; set; }
        public string err_description { get; set; }
        public string info { get; set; }
        public string ip { get; set; }
        public bool is_3ds { get; set; }
        public string liqpay_order_id { get; set; }
        public int mpi_eci { get; set; }
        public string order_id { get; set; }
        public int payment_id { get; set; }
        public string paytype { get; set; }
        public string public_key { get; set; }
        public decimal receiver_commission { get; set; }
        public string redirect_to { get; set; }
        public string refund_date_last { get; set; }
        public string rrn_credit { get; set; }
        public string rrn_debit { get; set; }
        public decimal sender_bonus { get; set; }
        public string sender_card_bank { get; set; }
        public string sender_card_country { get; set; }
        public string sender_card_mask2 { get; set; }
        public string sender_card_type { get; set; }
        public decimal sender_commission { get; set; }
        public string sender_first_name { get; set; }
        public string sender_last_name { get; set; }
        public string sender_phone { get; set; }
        public string status { get; set; }
        public string token { get; set; }
        public string type { get; set; }
        public decimal version { get; set; }
        public string err_erc { get; set; }
        public string product_category { get; set; }
        public string product_description { get; set; }
        public string product_name { get; set; }
        public string product_url { get; set; }
        public decimal refund_amount { get; set; }
        public string verifycode { get; set; }
    }
}