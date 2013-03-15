@..\tools\XmlCombiner\bin\Debug\XmlCombiner.exe ..\plugins | msxsl - populationList.xsl | ..\tools\XmlPP.exe > populationList.xml
@msxsl populationList.xml populationTable.xsl > populationList.html
