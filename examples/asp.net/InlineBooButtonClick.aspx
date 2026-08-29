<html>
    <script language="Boo" runat="server">
    def ButtonClick():
        Message.Text = Edit1.Text
    </script>
    <body>
    <form runat="server">
        <asp:textbox id="Edit1" runat="server"/>
        <asp:button text="Click Me!" OnClick="ButtonClick" runat="server"/>
    </form>
    <p><b><asp:label id="Message" runat="server"/></b></p>
    </body>
</html>