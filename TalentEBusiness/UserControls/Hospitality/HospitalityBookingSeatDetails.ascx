<%@ Control Language="VB" AutoEventWireup="false" CodeFile="HospitalityBookingSeatDetails.ascx.vb" Inherits="UserControls_HospitalityBookingSeatDetails" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityBookingApplyDiscount.ascx" TagName="HospitalityBookingApplyDiscount" TagPrefix="Talent" %>

<aside>
    <div class="c-hosp-bkng-s-deets">
        <div class="row">
            <div class="column">
                <div class="o-hosp-cont">
                    <Talent:HospitalityBookingApplyDiscount ID="HospitalityBookingApplyDiscount1" runat="server" />
                </div>
            </div>
        </div>
    </div>
</aside>
