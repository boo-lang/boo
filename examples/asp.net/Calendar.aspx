<script language="Boo" runat="server">
         /* 
	 Inline boo code is supported, as is -wsa mode ("end" instead of indenting):
	 
	 To enable whitespace agnostic mode, add this to the top of your asp page:
	 < %@ Page Language="Boo" CompilerOptions="-wsa" % >
	 */

def Calendar1Selected():
        Label1.Text = 'Boo for .NET says you picked ' + Calendar1.SelectedDate.ToString('D')

def Button1Click():
        Calendar1.VisibleDate = System.Convert.ToDateTime(Edit1.Text)
        Label1.Text = 'Boo for .NET says you set ' + Calendar1.VisibleDate.ToString('D')
	
</script>

<body style="font:18pt Verdana">
  <form runat="server">
    <center>
       <h1>Boo for .NET running in ASP.NET</h1>
       <p>Please pick a date</p>
       <asp:Calendar id="Calendar1" runat="server" ForeColor="#0000FF" BackColor="#FFFFCC" 
         OnSelectionChanged="Calendar1Selected">
         <TodayDayStyle Font-Bold="True"/>
         <NextPrevStyle ForeColor="#FFFFCC"/>
         <DayHeaderStyle BackColor="#FFCC66"/>
         <SelectedDayStyle ForeColor="Black" BackColor="#CCCCFF"/>
         <TitleStyle Font-Size="14pt" Font-Bold="True" ForeColor="#FFFFCC" BackColor="#990000"/>
         <OtherMonthDayStyle ForeColor="#CC9966"/>
       </asp:Calendar>
       <p><asp:TextBox id="Edit1" width=200 runat="server"/>
          <asp:Button text="Set date" id="Button1" OnClick="Button1Click" runat="server" />
       </p>
       <p><asp:Label id="Label1" runat="server"/></p>
    </center>
  </form>
</body>
