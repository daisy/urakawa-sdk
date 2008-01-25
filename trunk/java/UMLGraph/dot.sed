s/{Events = /{Events: /g
s/{Exceptions = /{Exceptions: /g
s/align="right"/align="left"/g
s/}Notes}//g
s/{Notes = {//g
s/node \[/node \[color=\"dimgray\",/g
s/edge \[/edge \[color=\"dimgray\",/g
s/taillabel="", label="", headlabel="", fontname="arialbd", fontcolor="firebrick3", fontsize=10.0, color="dimgray", arrowhead=open/taillabel="", label="", headlabel="", fontname="arialbd", fontcolor="firebrick3", fontsize=10.0, color="Gray", arrowhead=open/g
s/color="dimgray", arrowhead=open, style=dashed/color="darkgoldenrod1", arrowhead=open, style=dashed/g
s/label="Create", headlabel="", fontname="arialbd", fontcolor="firebrick3", fontsize=10.0, color="darkgoldenrod1", arrowhead=open/label="Create", headlabel="", fontname="arialbd", fontcolor="blue", fontsize=10.0, color="blue", arrowhead=open/g
s/label="Clone", headlabel="", fontname="arialbd", fontcolor="firebrick3", fontsize=10.0, color="darkgoldenrod1",/label="Clone", headlabel="", fontname="arialbd", fontcolor="honeydew4", fontsize=10.0, color="cornsilk4",/g
s/label="Aggregation", headlabel="1", fontname="arialbd", fontcolor="firebrick3", fontsize=10.0, color="darkgoldenrod1", arrowhead=open/label="Aggregation", headlabel="1", fontname="arialbd", fontcolor="darkgreen", fontsize=10.0, color="darkolivegreen3", arrowhead=open/g
s/label="Aggregation", headlabel="1..n", fontname="arialbd", fontcolor="firebrick3", fontsize=10.0, color="darkgoldenrod1", arrowhead=open/label="Aggregation", headlabel="1..n", fontname="arialbd", fontcolor="darkgreen", fontsize=10.0, color="darkolivegreen3", arrowhead=open/g
s/label="Aggregation", headlabel="0..n", fontname="arialbd", fontcolor="firebrick3", fontsize=10.0, color="darkgoldenrod1", arrowhead=open/label="Aggregation", headlabel="0..n", fontname="arialbd", fontcolor="darkgreen", fontsize=10.0, color="darkolivegreen3", arrowhead=open/g
s/<table border="0" cellspacing="0" cellpadding="1"><tr><td align="left">/<table border="0" cellspacing="0" cellpadding="1" cellborder="1"><tr><td align="left">/g
s/cellborder="1"><tr><td align="left">  <\/td>/><tr><td align="left">  <\/td>/g
s/laquo/lt/g
s/raquo/gt/g
s/OptionalDesignConvenienceInterface/_convenient_/g
s/OptionalLeafInterface/_sdk-implemented_/g
s/ApplicationImplemented/_app-implemented_/g
s/Language-Dependent/_language-dependent_/g
s/&lt;_/(/g
s/_&gt;/)/g
s/Initialize&gt; <\/td><\/tr><tr><td align="left">/Initialize\&gt; <br\/>/g
s/Abstract&gt; <\/td><\/tr><tr><td align="left">/Abstract\&gt; <br\/>/g
s/} <\/font><\/td>/} <\/td>/g
s/<\/td><\/tr><tr><td align="left"><font face="arial" point-size="8.0">/<br\/>/g
s/<\/td><\/tr><tr><td><font face="arialbd" point-size="10.0">/<br\/><font face="arialbd" point-size="10.0">/g
s/\+ \([^)<]*\)(/+ <font face="arialbd" point-size="10.0">\1<\/font>(/g
s/# \([^)<]*\)(^[h]/# <font face="arialbd" point-size="10.0">\1<\/font>(/g
s/- \([^)<]*\)(/- <font face="arialbd" point-size="10.0">\1<\/font>(/g
s/~ \([^)<]*\)(/~ <font face="arialbd" point-size="10.0">\1<\/font>(/g
s/\+ \([^:<]*\):/+ <font face="arialbd" point-size="10.0">\1<\/font>:/g
s/# \([^:<]*\):[^\/]/# <font face="arialbd" point-size="10.0">\1<\/font>:/g
s/- \([^:<]*\):/- <font face="arialbd" point-size="10.0">\1<\/font>:/g
s/~ \([^:>]*\):/~ <font face="arialbd" point-size="10.0">\1<\/font>:/g
s/Exceptions:/<font color="red">Exceptions:<\/font>/g
s/Events:/<font color="blue">Events:<\/font>/g
s/align="left"/align="left" balign="left"/g
s/<br\/>/<br align="left"\/>/g
s/) <\/td><\/tr><tr><td> &/)<br\/>\&/g
s/&gt; <\/td><\/tr><tr><td> (/\&gt;<br\/>(/g
s/&gt; <\/td><\/tr><tr><td> &/\&gt;<br\/>\&/g
s/cellpadding="2"/cellpadding="0"/g
s/cellpadding="1"/cellpadding="4"/g