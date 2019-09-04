# Prover
Verify and approve Electronic Volume Correctors for Measurement Canada accreditation sealing.

Automated testing for rotary, mechanical and frequency inputs.

Devices Supported:

- Honeywell/Mercury Instruments
  - Mini-Max
  - Mini-AT
  - EC-350
  - TCI
  - Turbo Corrector

## Install Belkin USB-Serial adapter - Windows 10
As of Windows 10, drivers must be digitally signed. Unfortunately, there's no easy way around this.
Fortunately, you can create your own signing certificate locally and sign them yourself.

* Download drivers from here: http://www.clearchain.com/blog/posts/how-to-use-a-belkin-f5u409f5u409-cu-usb-to-pda-serial-converter-in-visawindows-7-64bit-mac-osx-10
* Follow this for the above files https://technet.microsoft.com/en-us/library/dd919238(v=ws.10).aspx

## Install Measurement Computing USB board

The board needs CB.cfg file for it to work. To have it generate start InstaCal from the MccDAQ application suite. Follow the instructions (if there's any).

http://www.mccdaq.com/DOWNLOADS/example_programs/Universal_Library_Apps_and_App_Notes/AppNotes/How_do_multiple_apps_and_devices,_without_resetting_a_USB_already_running_in_an_application.pdf
