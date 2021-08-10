<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Banner.ascx.cs" Inherits="PageToSync_UserControls_Banner" %>

<div class="details-banner">
    <div class="details-banner--overlay"></div>
    <div class="details-banner--hero">
        <img runat="server" class="details-banner--hero--img" id="heroBannerImage"/>
    </div>
    <div class="details-banner--info">
        <a href="#" class="details-banner--info--box">
            <img runat="server" Id="detailsBannerImage" class="details-banner--info--box--img"/>
        </a>
        <h1 runat="server" class="details-banner--info--name"><a runat="server" Id="detailsName" class="details-banner--info--name--link"></a></h1>
    </div>
</div>