# DatafeedTest
This application is for lazy processing large XML files. This XMLs should contains serialized list of objects. 
1. Large XML is lazy loaded into smaller valid XML's. Split is done on the Root-node level.
2. Smaller XML's are deserialized with XMLSerialization into classes generated with XSD.exe from XSD files. 
3. XSD Classes are mapped to ORM classes.
4. ORM classes are saved into virtual database.
