### DatafeedTest
Test-only console application.

This application is for lazy processing large XML files (imagine hundreds of gigabajts - something which will not fit into RAM). This XMLs should contains serialized list of objects. 
1. Large XML is lazy loaded into smaller valid XML's. Split is done on the Root-node level. This smaller XML files are saved to disc.
2. Smaller XML's are one by one automatically deserialized with XMLSerialization into classes generated with XSD.exe from XSD files. 
3. XSD Classes are mapped to ORM classes.
4. ORM classes are saved into simulated database. If small XML is processed completely and commited to DB, then it is removed from disc. Otherwise it will be still there for future manual handling of occured error.

Main design of this app is to provide generic processing for any XML file containing serialized list of object. Therefore there is only a few manual coding for any datafeed file added in the future.
