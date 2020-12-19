![Reanimator gloss](/images/ReanimatorGloss.jpg)

**The world is full of legacy software, running all types of businesses in all types of industries.**

In our personal lives we're all accustomed to smartphones, tablets, and cloud services; but if you step behind the counter in one of those businesses and look at the dusty 14 inch CRT used by the hourly employees, you just might see something like this:

![WDS-II screen capture](/images/WDS_II_example.jpg)

Sure, all those employees have smartphones for their personal use.  Some of them are running business apps on their personal phones.  They have other computers in the back office, running web-enabled Windows applications.  **And yet** the software that tracks sales, inventory, accounting and payroll may not have changed in 20 or 30 years.

**But why?**  Why haven't they ripped out that old stuff and replaced it with something modern?

The business owner probably chose that software package decades ago when it was state-of-the-art.  The business has grown up around that software.  The staff are trained and experienced in its use; and it's tightly integrated into their supply chain.  Most important, that software contains the mission-critical data that keeps the business running.  Replacing it would be costly and disruptive.  To the owner, ripping out that "obsolete" software and replacing it looks like a huge, expensive headache.

At the same time, the management team is well aware that technology has moved on.  It would be great if they could integrate that old software with other, modern systems -- Ecommerce, mobile devices, web services, etc -- but **they just don't know how to do it**, because until now it hasn't been possible.

# Reanimator

What if that business had a software adapter layer that could operate the user interface of their legacy software, as if that old software had a rich developer API -- extracting data and exporting it to other systems, performing the same functions that employees do when they use the software?  That's what **Reanimator** does.

You can use that ability to put new GUI skins on top of old functionalty, changing workflow or just updating the look and feel.  You can add service endpoints to the old software, so its functionalty can be accessed through web portals, mobile devices, or whatever.  You can mine the data in the old system, and present it to users in a new way.

Here are some examples of applications I've done with a legacy system called WDS-II:

### Point of Sale

[![Point of Sale](http://img.youtube.com/vi/3t3N_ouGeCk/0.jpg)](http://www.youtube.com/watch?v=3t3N_ouGeCk "Point of Sale")

### Counting inventory with speech recognition

[![Counting Inventory](http://img.youtube.com/vi/ziy9DvCxrRc/0.jpg)](http://www.youtube.com/watch?v=ziy9DvCxrRc "Counting Inventory")

### Managing Stock Lists

[![Managing Stock Lists](http://img.youtube.com/vi/2bcLmyh1NrA/0.jpg)](http://www.youtube.com/watch?v=2bcLmyh1NrA "Managing Stock Lists")

### Product Lookup From a Mobile Device

[![Product Lookup From a Mobile Device](http://img.youtube.com/vi/tah2IuiGp5Q/0.jpg)](http://www.youtube.com/watch?v=tah2IuiGp5Q "Product Lookup From a Mobile Device")

### What Else Is Going On In This Market?

I haven't found a way to search Google for **"legacy green-screen software systems and the companies that use them."**  To me this seems like the core of the sales problem:  finding out what those systems are, and which companies are still using them.

Having worked with WDS-II, I do know something about the ecosystem surrounding it.  Users of that system are supported by a handful of consulting companies that offer three things to their customers:
1. Tech support for WDS-II;
2. Updates to the language and database environment;
3. Contract development services to rewrite applications.

The companies I know of that offer WDS-II support are:
* [Allegro Consultants](https://allegroconsultants.com/erp-applications/wds-ii)
* [Integrated Business Technologies](http://www.ibtechs.com/)
* [MD Pro Systems](https://www.mdprosystems.com/)

[Zebra Technologies](https://www.zebra.com/us/en.html) specializes in barcode scanners, and has a lot of customers in the warehousing / distribution business.  In an effort to integrate modern mobile devices into this industry, they've introduced a product that lets you operate a legacy software system from an Android device:

[![Zebra All-Touch TE](http://img.youtube.com/vi/gx-21ga6hYQ/0.jpg)](http://www.youtube.com/watch?v=gx-21ga6hYQ "Zebra All-Touch TE")

This Zebra product is an improvement, but it just presents the same old green-screen user interface on a mobile device.  With Reanimator, it's possible to do much better.

Quoting Daniel Park in the Zebra video:  "We've found that our warehousing and DC (distribution center) customers have been slower to move to modern Android devices.  Generally it's because they don't want to touch their back-end warehouse management system."

In all these cases, users are trapped between two options they don't like:
1.  Keep doing things the same way they've always been done; or
2.  Rewrite the old software at great expense

### The Consulting Business Model

Business owners who can benefit from Reanimator are not interested in hiring a software team to do an integration with their legacy system.  They need somebody to do it for them.

The Reanimator software framework is finished, tested, and ready to go.  **All it needs now is customers.**  I don't have the skills or the desire to do marketing and sales, but if you do and you're interested, please get in touch!

Integrating Reanimator with a new legacy system involves writing some code, but it does **not** require the people writing that code to know all the details of the framework underneath.  They only need to be competent with .NET and C#.  If a marketing team were able to generate enough business to require more development manpower, it would be relatively easy to hire those people or contract out the integration work.

The [technical section](technical.md) gives a fairly detailed explanation of the type of code that must be written to do a new integration.

### The Software Tool Business Model

The process of doing new integrations could be made even easier by turning Reanimator into a Visual Studio VSIX extension, which could be sold to developers who want to "franchise" the integration process.  I've started work on this extension, but it's not ready to be shipped.

### The Technology Behind Reanimator

If you're a software person and you want to know more about how Reanimator works, you can check out the [technical section](technical.md) of this site.

