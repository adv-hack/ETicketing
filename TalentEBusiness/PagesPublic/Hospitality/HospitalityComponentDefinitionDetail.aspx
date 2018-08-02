<%@ Page Language="VB" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" CodeFile="HospitalityComponentDefinitionDetail.aspx.vb" Inherits="PagesPublic_Hospitality_HospitalityComponentDefinitionDetails" ViewStateMode="Disabled" %>

<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityComponentDefinitionDetail.ascx" TagName="HospitalityComponentDefinitionDetail" TagPrefix="Talent" %>

<asp:content id="ContentHead" contentplaceholderid="ContentPlaceHolder2" runat="server"></asp:content>
<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <div class="row">
        <div class="columns small-12 medium-8 large-9">
            <Talent:HospitalityComponentDefinitionDetail ID="HospitalityComponentDefinitionDetail" runat="server" />
            <div class="panel">
                <h2>163 Seats&hellip;</h2>
                <table>
                    <thead>
                        <tr>
                            <th class="no-sort">
                                <input type="checkbox" name="component" value="all"></th>
                            <th>Stand</th>
                            <th>Area</th>
                            <th>Row</th>
                            <th>Seat</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td><input type="checkbox" name="component" value="a"></td>
                            <td>CAR Car Parking</td>
                            <td>LVL1 Level 1 Car Parking</td>
                            <td>A</td>
                            <td>0003</td>
                        </tr>
                        <tr>
                            <td><input type="checkbox" name="component" value="a"></td>
                            <td>CAR Car Parking</td>
                            <td>LVL1 Level 1 Car Parking</td>
                            <td>A</td>
                            <td>0004</td>
                        </tr>
                        <tr>
                            <td><input type="checkbox" name="component" value="a"></td>
                            <td>CAR Car Parking</td>
                            <td>LVL1 Level 1 Car Parking</td>
                            <td>A</td>
                            <td>0005</td>
                        </tr>
                        <tr>
                            <td><input type="checkbox" name="component" value="a"></td>
                            <td>CAR Car Parking</td>
                            <td>LVL1 Level 1 Car Parking</td>
                            <td>A</td>
                            <td>0006</td>
                        </tr>
                        <tr>
                            <td><input type="checkbox" name="component" value="a"></td>
                            <td>CAR Car Parking</td>
                            <td>LVL1 Level 1 Car Parking</td>
                            <td>A</td>
                            <td>0007</td>
                        </tr>
                        <tr>
                            <td><input type="checkbox" name="component" value="a"></td>
                            <td>CAR Car Parking</td>
                            <td>LVL1 Level 1 Car Parking</td>
                            <td>A</td>
                            <td>0002</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <div class="columns small-12 medium-4 large-3">
            <div class="c-hosp-comp-def-deets-ad-s js-hosp-comp-def-deets-ad-s--not-required" >
                <div class="panel">
                    <h2>Add Seat(s)&hellip;</h2>
                    <div class="alert-box warning c-hosp-comp-def-deets-ad-s__not-required">
                        &quot;Ticket Required&quot;
                    </div>
                    <div class="c-hosp-comp-def-deets-ad-s__required">
                        <div class="c-hosp-comp-def-deets-ad-s__stand">
                            <label>Stand</label>
                            <select>
                                <option value="volvo">Select Stand</option>
                                <option value="saab">Saab</option>
                                <option value="mercedes">Mercedes</option>
                                <option value="audi">Audi</option>
                            </select>
                        </div>
                        <div class="c-hosp-comp-def-deets-ad-s__area">
                            <label>Area</label>
                            <select>
                                <option value="volvo">Select Areas</option>
                                <option value="saab">Saab</option>
                                <option value="mercedes">Mercedes</option>
                                <option value="audi">Audi</option>
                            </select>
                        </div>
                        <div class="c-hosp-comp-def-deets-ad-s__row">
                            <label>Row</label>
                            <input type="text" name="row" placeholder="Row">
                        </div>
                        <div class="c-hosp-comp-def-deets-ad-s__seat">
                            <label>Seat</label>
                            <input type="text" name="seat" placeholder="Seat">
                        </div>
                        <div class="c-hosp-comp-def-deets-ad-s__suffix">
                            <label>Suffix</label>
                            <input type="text" name="Suffix" placeholder="Suffix">
                        </div>
                        <div class="c-hosp-comp-def-deets-ad-s__add">
                            <a href="#" class="button">Add</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="2" />
</asp:content>