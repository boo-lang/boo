total = { return system.QueryTotalDayActivity(date.Today) }
print "You have already worked ${total().TotalHours} today."
