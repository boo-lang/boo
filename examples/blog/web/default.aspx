<%@ Page Inherits="BooLog.Web.DefaultPage" %>
<html>
<body>
<form runat="server">

<asp:Repeater id="_entries" runat="server">
<ItemTemplate>

<h2><%#GetEntryTitle(Container.DataItem)%></h2>
<p><%#GetEntryBody(Container.DataItem)%></p>

</ItemTemplate>
</asp:Repeater>

</form>
</body>
</html>
