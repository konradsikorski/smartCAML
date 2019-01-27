## Changelog

### v. 2.3
##### Fix
+ 'is empty'/'not empty' operator should not generate 'value' xml element

### v. 2.2
##### Update
+ load lookup values in paging
##### Fix
+ loading lookup dropdown values in designer cause an exception
+ In operator in designer was wrongly initialized with value

### v. 2.0
##### New
+ Query designer can be build from xml view
+ Page size for caml query
+ UserID as a value for User fields

##### Update
+ about window as popup
+ columns settings window as a popup

##### Fix
+ placeholder for 'site url' dropdown is now working when it is empty on start

### v. 1.9
##### New
+ Lookup and User fields now loads available options to dropdown
+ Now SmartCAML is also available in Windows Store!
##### Update
+ Connect window is now as popup not a dialog
+ Redesigned connect window
+ .NET 4.6.1 is now required

### v. 1.8.6
##### Update
+ Lookup field dropdown in designer now load items
+ User field dropdown in designer now load items

### v. 1.8.6
##### Update
+ 'Inlude time' option available in caml designer for DateTime fields
+ Change the description for API section in Connect window
+ Read order by settings from xml to designer
+ New options for items grid header context menu: "Hide all hidden" and "Hide all readonly"
+ New options for items grid header context menu: "Pin" and "Unpin"

##### Fix
+ Change the watermark for password input in Connect window
+ Down arrow used to order query was not working

### v. 1.8.5
##### New
+ User column has now "lookup id" dropdown
+ pasing text to Xml tab will be formated as nice xml
+ new certificate for publishing clickonce because old expired
##### Fix
+ boolen column uses User designer and vice versa

### v. 1.8.3
##### New
+ Keep user settings after app upgrade
+ Add link to logs file in About window
##### Fix
+ Generating User Id
+ Service address

### v. 1.8.0
##### New
+ Application icon and logo
+ Send update and install information to the server
+ Support 'Membership' operator
+ Support 'In' Operator
##### Fix
+ Display DateTime values in local time zone
+ Updating DateTime field now will no longer crach