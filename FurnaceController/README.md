# DS1302

> A library for the DS1302 RTC Module

## How to use it

Just follow the `example.ino` file, everything is detailed there !

## PINS

* RST : **11**
* DAT : **12**
* CLK : **13**

### Access variables

You can access these variables :

`rtc.dateTime.Seconds10`, `rtc.dateTime.Seconds`  
`rtc.dateTime.Minutes10`, `rtc.dateTime.Minutes`  
`rtc.dateTime.h24.Hour10`, `rtc.dateTime.h24.Hour`  
`rtc.dateTime.h12.Hour10`, `rtc.dateTime.h12.Hour`  
`rtc.dateTime.Date10`, `rtc.dateTime.Date`  
`rtc.dateTime.Month10`, `rtc.dateTime.Month`  
`rtc.dateTime.Year10`, `rtc.dateTime.Year`  
`rtc.dateTime.Day`

... after having updated the buffer with :

```cpp
rtc.read();
```
