# ProjectUoMTimetable
## Background
I've noticed a lot of Melbourne Uni students use their timetable as background / lockscreen wallpaper of their devices. However, chances are, some classes are not available every week, which will make the timetable of every week different. This project is meant to subscribe your timetable and generate pictures of each week so that you can use them as wallpaper.
It also helps if you travel a lot (it will dynamically adjust according to local time zone).

## Development
The project is currently under development. I know it's really difficult to read the code at the moment but I'm improving it. My first sense is to get it up and running, and then, decouple the code, add descriptions, and improve human-computer interaction.

## Usability
The project is still in testing, and is currently written all in Console application. It does not have any GUI, which however will be available in the future (of course).

## Platform
The project is meant to be able to run on Android / Windows / Linux / macOS.
However, at the moment, the project is developed under Linux w/ .NET 5.0, which means, you can clone the code and get it running on Windows / Linux / macOS straight away (```dotnet run```) but not yet for Android.

## Short-sighted Plans
#### Deserialisation & Subscription
- [ ] Deserialise .ics files
- [x] Read from online subscription link
- [ ] Check updates (auto/manual) (prob by DTSTAMP)
#### Generator & Wallpaper Service
- [x] Generate .png by a specific "week"
- [ ] Automatically set the .png as wallpaper (background/lockscreen)
#### Customisation
- [ ] Import config file (.xml)
- [ ] Export config file (or a standalone style designer)
- [ ] Custom top/left margin
- [ ] Custom fontStyle/fontSize
- [ ] Custom alignment of each section
- [ ] Custom background (solid/preset/custom)
- [ ] Custom lineStyle/lineColor
- [ ] Custom eventStyle/eventColor
- [ ] Show/Hide week of day indicator
- [ ] Custom monthStyle (number/short/long)
- [ ] Custom timeStyle (24hr/12hr) (with colon/no colon)
#### Globalisation
##### Timezone
- [x] Work with different time zone (no split).
- [ ] Work with time zones that splits one event into two days (e.g. dayA:11:30 pm - dayB:12:30 am)
##### Language
- [x] en-AU
- [ ] zh-CN
