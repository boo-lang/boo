<%@ Page Inherits="BooLog.Web.Admin.DefaultPage" %>
<html>
<body>

<form runat="server">

<asp:DataGrid id="_entries" runat="server" AutoGenerateColumns="False"
		Width="70%" HorizontalAlign="center">

<Columns>

<asp:TemplateColumn HeaderText="Posted">
<ItemTemplate><%#GetDatePosted(Container.DataItem)%></ItemTemplate>
</asp:TemplateColumn>

<asp:TemplateColumn HeaderText="Title">
<ItemTemplate><%#GetTitle(Container.DataItem)%></ItemTemplate>
</asp:TemplateColumn>

</Columns>

</asp:DataGrid>

</form>
</body>
</html>
