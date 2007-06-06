s/Exceptions = /Exceptions: /g
s/align="right"/align="center"/g
s/}Notes}//g
s/{Notes = {//g
s/node \[/node \[color=\"dimgray\",/g
s/edge \[/edge \[color=\"dimgray\",/g
s/taillabel="", label="", headlabel="", fontname="arialbd", fontcolor="firebrick3", fontsize=10.0, color="dimgray", arrowhead=open/taillabel="", label="", headlabel="", fontname="arialbd", fontcolor="firebrick3", fontsize=10.0, color="Gray", arrowhead=open/g
s/color="dimgray", arrowhead=open, style=dashed/color="darkgoldenrod1", arrowhead=open, style=dashed/g
s/label="Create", headlabel="", fontname="arialbd", fontcolor="firebrick3", fontsize=10.0, color="darkgoldenrod1", arrowhead=open/label="Create", headlabel="", fontname="arialbd", fontcolor="blue", fontsize=10.0, color="blue", arrowhead=open/g
s/label="Clone", headlabel="", fontname="arialbd", fontcolor="firebrick3", fontsize=10.0, color="darkgoldenrod1",/label="Clone", headlabel="", fontname="arialbd", fontcolor="honeydew4", fontsize=10.0, color="cornsilk4",/g
s/label="Aggregation", headlabel="1", fontname="arialbd", fontcolor="firebrick3", fontsize=10.0, color="darkgoldenrod1", arrowhead=open/label="Aggregation", headlabel="1", fontname="arialbd", fontcolor="darkgreen", fontsize=10.0, color="darkolivegreen3", arrowhead=open/g
s/label="Aggregation", headlabel="1..n", fontname="arialbd", fontcolor="firebrick3", fontsize=10.0, color="darkgoldenrod1", arrowhead=open/label="Aggregation", headlabel="1", fontname="arialbd", fontcolor="darkgreen", fontsize=10.0, color="darkolivegreen3", arrowhead=open/g
s/label="Aggregation", headlabel="0..n", fontname="arialbd", fontcolor="firebrick3", fontsize=10.0, color="darkgoldenrod1", arrowhead=open/label="Aggregation", headlabel="1", fontname="arialbd", fontcolor="darkgreen", fontsize=10.0, color="darkolivegreen3", arrowhead=open/g
s/&laquo;Initialize/<br\/> \&laquo;Initialize/g
s/&laquo;Abstract/<br\/> \&laquo;Abstract/g
s/OptionalDesignConvenienceInterface/_convenience-only_/g
s/OptionalLeafInterface/_sdk-implemented_/g
s/ApplicationImplemented/_app-implemented_/g
s/Language-Dependent/_language-dependent_/g
s/} <\/font/} <br\/> <\/font/g
s/laquo/lt/g
s/raquo/gt/g
s/&lt;_/(/g
s/_&gt; /) /g
