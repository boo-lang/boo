<%@ Page Inherits="BooLog.Web.Admin.PostPage" %>
<html>
<body>
<form runat="server">

<center>

<div style="width: 70%">

Title: <asp:TextBox runat="server" id="_title" Columns="60" />
<asp:RequiredFieldValidator runat="server" ErrorMessage="*" ControlToValidate="_title" />
<br />
<asp:TextBox runat="server" id="_body" Columns="80" Rows="40" TextMode="MultiLine" />
<br />
<asp:RequiredFieldValidator runat="server" ErrorMessage="*" ControlToValidate="_body" />
<br />
<asp:Button id="_post" runat="server" Text="Post" OnClick="_post_Click" />

</div>

</center>

</form>
</body>
</html>
