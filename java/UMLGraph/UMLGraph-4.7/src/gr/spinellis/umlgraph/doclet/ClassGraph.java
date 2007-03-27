/*
 * Create a graphviz graph based on the classes in the specified java
 * source files.
 *
 * (C) Copyright 2002-2005 Diomidis Spinellis
 *
 * Permission to use, copy, and distribute this software and its
 * documentation for any purpose and without fee is hereby granted,
 * provided that the above copyright notice appear in all copies and that
 * both that copyright notice and this permission notice appear in
 * supporting documentation.
 *
 * THIS SOFTWARE IS PROVIDED ``AS IS'' AND WITHOUT ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED WARRANTIES OF
 * MERCHANTIBILITY AND FITNESS FOR A PARTICULAR PURPOSE.
 *
 * $Id: ClassGraph.java,v 1.85 2007/03/26 07:04:06 dds Exp $
 *
 */

package gr.spinellis.umlgraph.doclet;

import java.io.BufferedOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.regex.Pattern;

import com.sun.javadoc.ClassDoc;
import com.sun.javadoc.ConstructorDoc;
import com.sun.javadoc.Doc;
import com.sun.javadoc.FieldDoc;
import com.sun.javadoc.MethodDoc;
import com.sun.javadoc.PackageDoc;
import com.sun.javadoc.Parameter;
import com.sun.javadoc.ParameterizedType;
import com.sun.javadoc.ProgramElementDoc;
import com.sun.javadoc.RootDoc;
import com.sun.javadoc.Tag;
import com.sun.javadoc.Type;
import com.sun.javadoc.TypeVariable;
import com.sun.javadoc.WildcardType;

/**
 * Class graph generation engine
 * @depend - - - StringUtil
 * @depend - - - Options
 * @composed - - * ClassInfo
 * @has - - - OptionProvider
 *
 * @version $Revision: 1.85 $
 * @author <a href="http://www.spinellis.gr">Diomidis Spinellis</a>
 */
class ClassGraph {
    protected static final char FILE_SEPARATOR = '/';
    enum Font {
	NORMAL, ABSTRACT, CLASS, CLASS_ABSTRACT, TAG, PACKAGE
    }
    enum Align {
	LEFT, CENTER, RIGHT
    };
    public static Map<RelationType, String> associationMap = new HashMap<RelationType, String>();
    static {
	associationMap.put(RelationType.ASSOC, "arrowhead=none");
	associationMap.put(RelationType.NAVASSOC, "arrowhead=open");
	associationMap.put(RelationType.HAS, "arrowhead=none, arrowtail=ediamond");
	associationMap.put(RelationType.COMPOSED, "arrowhead=none, arrowtail=diamond");
	associationMap.put(RelationType.DEPEND, "arrowhead=open, style=dashed");
    }
    protected Map<String, ClassInfo> classnames = new HashMap<String, ClassInfo>();
    protected Set<String> rootClasses;
    protected OptionProvider optionProvider;
    protected PrintWriter w;
    protected ClassDoc collectionClassDoc;
    protected ClassDoc mapClassDoc;
    protected String linePostfix;
    protected String linePrefix;
    
    // used only when generating context class diagrams in UMLDoc, to generate the proper
    // relative links to other classes in the image map
    protected Doc contextDoc;
      
    /**
     * Create a new ClassGraph.  <p>The packages passed as an
     * argument are the ones specified on the command line.</p>
     * <p>Local URLs will be generated for these packages.</p>
     * @param root The root of docs as provided by the javadoc API
     * @param optionProvider The main option provider
     * @param contextDoc The current context for generating relative links, may be a ClassDoc 
     * 	or a PackageDoc (used by UMLDoc)
     */
    public ClassGraph(RootDoc root, OptionProvider optionProvider, Doc contextDoc) {
	this.optionProvider = optionProvider;
	this.collectionClassDoc = root.classNamed("java.util.Collection");
	this.mapClassDoc = root.classNamed("java.util.Map");
	this.contextDoc = contextDoc;
	
	// to gather the packages containing specified classes, loop thru them and gather
	// package definitions. User root.specifiedPackages is not safe, since the user
	// may specify just a list of classes (human users usually don't, but automated tools do)
	rootClasses = new HashSet<String>();
	for (ClassDoc classDoc : root.classes()) 
	    rootClasses.add(classDoc.qualifiedName());
	
	Options opt = optionProvider.getGlobalOptions();
	if (opt.compact) {
	    linePrefix = "";
	    linePostfix = "";
	} else {
	    linePrefix = "\t";
	    linePostfix = "\n";
	}
    }

    

    /** Return the class's name, possibly by stripping the leading path */
    private String qualifiedName(Options opt, String r) {
	if (!opt.showQualified) {
	    // Create readable string by stripping leading path
	    for (;;)  {
		int dotpos = r.lastIndexOf('.');
		if (dotpos == -1) break; // Work done!
		/*
		 * Change all occurences of
		 * "p1.p2.myClass<S extends dummy.Otherclass>" into
		 * "myClass<S extends Otherclass>"
		 */
		int start = dotpos;
		while (start > 0 && Character.isJavaIdentifierPart(r.charAt(start - 1)))
		    start--;
		r = r.substring(0, start) + r.substring(dotpos + 1);
	    }
	}
	return r;
    }

    /**
     * Escape &lt;, &gt;, and &amp; characters in the string with
     * the corresponding HTML entity code.
     */
    private String escape(String s) {
	final Pattern toEscape = Pattern.compile("[&<>]");

	if (toEscape.matcher(s).find()) {
	    StringBuffer sb = new StringBuffer(s);
	    for (int i = 0; i < sb.length();) {
		switch (sb.charAt(i)) {
		case '&':
		    sb.replace(i, i + 1, "&amp;");
		    i += "&amp;".length();
		    break;
		case '<':
		    sb.replace(i, i + 1, "&lt;");
		    i += "&lt;".length();
		    break;
		case '>':
		    sb.replace(i, i + 1, "&gt;");
		    i += "&gt;".length();
		    break;
		default:
		    i++;
		}
	    }
	    return sb.toString();
	} else
	    return s;
    }


    /**
     * Convert &lt; and &gt; characters in the string to the respective guillemot characters.
     */
    private String guillemize(Options opt, String s) {
	StringBuffer r = new StringBuffer(s);
	for (int i = 0; i < r.length();)
	    switch (r.charAt(i)) {
	    case '<':
		r.replace(i, i + 1, opt.guilOpen);
		i += opt.guilOpen.length();
		break;
	    case '>':
		r.replace(i, i + 1, opt.guilClose);
		i += opt.guilClose.length();
		break;
	    default:
		i++;
		break;
	    }
	return r.toString();
    }

    /**
     * Wraps a string in Guillemot (or an ASCII substitute) characters.
     *
     * @param str the <code>String</code> to be wrapped.
     * @return the wrapped <code>String</code>.
     */
    private String guilWrap(Options opt, String str) {
	return opt.guilOpen + str + opt.guilClose;
    }

    /**
     * Print the visibility adornment of element e prefixed by
     * any stereotypes
     */
    private String visibility(Options opt, ProgramElementDoc e) {
	if (!opt.showVisibility)
	    return " ";
	if (e.isPrivate())
	    return "- ";
	else if (e.isPublic())
	    return "+ ";
	else if (e.isProtected())
	    return "# ";
	else if (e.isPackagePrivate())
	    return "~ ";
	else
	    return " ";
    }

    /** Print the method parameter p */
    private String parameter(Options opt, Parameter p[]) {
	String par = "";
	for (int i = 0; i < p.length; i++) {
	    par += p[i].name() + typeAnnotation(opt, p[i].type());
	    if (i + 1 < p.length)
		par += ", ";
	}
	return par;
    }

    /** Print a a basic type t */
    private String type(Options opt, Type t) {
	String type = "";
	if (opt.showQualified)
	    type = t.qualifiedTypeName();
	else
	    type = t.typeName();
	type += typeParameters(opt, t.asParameterizedType());
	return type;
    }

    /** Print the parameters of the parameterized type t */
    private String typeParameters(Options opt, ParameterizedType t) {
	String tp = "";
	if (t == null)
	    return tp;
	Type args[] = t.typeArguments();
	tp += "&lt;";
	for (int i = 0; i < args.length; i++) {
	    tp += type(opt, args[i]);
	    if (i != args.length - 1)
		tp += ", ";
	}
	tp += "&gt;";
	return tp;
    }

    /** Annotate an field/argument with its type t */
    private String typeAnnotation(Options opt, Type t) {
	String ta = "";
	if (t.typeName().equals("void"))
	    return ta;
	ta += " : ";
	ta += type(opt, t);
	ta += t.dimension();
	return ta;
    }

    /** Print the class's attributes fd */
    private void attributes(Options opt, FieldDoc fd[]) {
	for (FieldDoc f : fd) {
	    if (hidden(f))
		continue;
	    String att = "";
	    stereotype(opt, f, Align.LEFT);
	    att = visibility(opt, f) + f.name();
	    if (opt.showType)
		att += typeAnnotation(opt, f.type());
	    tableLine(Align.LEFT, att);
	    tagvalue(opt, f);
	}
    }

    /*
     * The following two methods look similar, but can't
     * be refactored into one, because their common interface,
     * ExecutableMemberDoc, doesn't support returnType for ctors.
     */

    /** Print the class's constructors m */
    private boolean operations(Options opt, ConstructorDoc m[]) {
	boolean printed = false;
	for (ConstructorDoc cd : m) {
	    if (hidden(cd))
		continue;
	    stereotype(opt, cd, Align.LEFT);
	    String cs = visibility(opt, cd) + cd.name();
	    if (opt.showType) {
		cs += "(" + parameter(opt, cd.parameters()) + ")";
	    } else {
		cs += "()";
	    }
	    tableLine(Align.LEFT, cs);
	    printed = true;
	    tagvalue(opt, cd);
	}
	return printed;
    }

    /** Print the class's operations m */
    private boolean operations(Options opt, MethodDoc m[]) {
	boolean printed = false;
	for (MethodDoc md : m) {
	    if (hidden(md))
		continue;
	    // Filter-out static initializer method
	    if (md.name().equals("<clinit>") && md.isStatic() && md.isPackagePrivate())
		continue;
	    stereotype(opt, md, Align.LEFT);
	    String op = visibility(opt, md) + md.name();
	    if (opt.showType) {
		op += "(" + parameter(opt, md.parameters()) + ")"
			+ typeAnnotation(opt, md.returnType());
	    } else {
		op += "()";
	    }
	    tableLine(Align.LEFT, op, opt, md.isAbstract() ? Font.ABSTRACT : Font.NORMAL);
	    printed = true;

	    tagvalue(opt, md);
	}
	return printed;
    }

    /** Print the common class node's properties */
    private void nodeProperties(Options opt) {
	w.print(", fontname=\"" + opt.nodeFontName + "\"");
	w.print(", fontcolor=\"" + opt.nodeFontColor + "\"");
	w.print(", fontsize=" + opt.nodeFontSize);
	w.println("];");
    }

    /**
     * Return as a string the tagged values associated with c
     * @param opt the Options used to guess font names
     * @param c the Doc entry to look for @tagvalue
     * @param prevterm the termination string for the previous element
     * @param term the termination character for each tagged value
     */
    private void tagvalue(Options opt, Doc c) {
	Tag tags[] = c.tags("tagvalue");
	if (tags.length == 0)
	    return;
	
	for (Tag tag : tags) {
	    String t[] = StringUtil.tokenize(tag.text());
	    if (t.length != 2) {
		System.err.println("@tagvalue expects two fields: " + tag.text());
		continue;
	    }
	    tableLine(Align.RIGHT, "{" + t[0] + " = " + t[1] + "}", opt, Font.TAG);
	}
    }

    /**
     * Return as a string the stereotypes associated with c
     * terminated by the escape character term
     */
    private void stereotype(Options opt, Doc c, Align align) {
	for (Tag tag : c.tags("stereotype")) {
	    String t[] = StringUtil.tokenize(tag.text());
	    if (t.length != 1) {
		System.err.println("@stereotype expects one field: " + tag.text());
		continue;
	    }
	    tableLine(align, guilWrap(opt, t[0]));
	}
    }

    /** Return true if c has a @hidden tag associated with it */
    private boolean hidden(ProgramElementDoc c) {
	Tag tags[] = c.tags("hidden");
	if (tags.length > 0)
	    return true;
	tags = c.tags("view");
	if (tags.length > 0)
	    return true;
    Options opt; 
    if(c instanceof ClassDoc)
      opt = optionProvider.getOptionsFor((ClassDoc) c);
    else 
      opt = optionProvider.getOptionsFor(c.containingClass());
	return opt.matchesHideExpression(c.toString());
    }
    
    protected ClassInfo getClassInfo(String className) {
	return classnames.get(removeTemplate(className));
    }
    
    private ClassInfo newClassInfo(String className, boolean printed, boolean hidden) {
	ClassInfo ci = new ClassInfo(printed, hidden);
        classnames.put(removeTemplate(className), ci);
        return ci;
    }

    /** Return true if the class name is associated to an hidden class or matches a hide expression */
    private boolean hidden(String s) {
	ClassInfo ci = getClassInfo(s);
	Options opt = optionProvider.getOptionsFor(s);
	if(ci != null)
	    return ci.hidden || opt.matchesHideExpression(s);
	else
	    return opt.matchesHideExpression(s);
    }

    

    /**
     * Prints the class if needed.
     * <p>
     * A class is a rootClass if it's included among the classes returned by
     * RootDoc.classes(), this information is used to properly compute
     * relative links in diagrams for UMLDoc
     */
    public String printClass(ClassDoc c, boolean rootClass) {
	ClassInfo ci;
	boolean toPrint;
	Options opt = optionProvider.getOptionsFor(c);

	String className = c.toString();
	if ((ci = getClassInfo(className)) != null)
	    toPrint = !ci.nodePrinted;
	else {
	    toPrint = true;
	    ci = newClassInfo(className, true, hidden(c));
	}
	if (toPrint && !hidden(c) && (!c.isEnum() || opt.showEnumerations)) {
	    // Associate classname's alias
	    String r = className;
	    w.println("\t// " + r);
	    // Create label
	    w.print("\t" + ci.name + " [label=");
	    externalTableStart(opt, c.qualifiedName(), classToUrl(c, rootClass));
	    innerTableStart();
	    if (c.isInterface())
		tableLine(Align.CENTER, guilWrap(opt, "interface"));
	    if (c.isEnum())
		tableLine(Align.CENTER, guilWrap(opt, "enumeration"));
	    stereotype(opt, c, Align.CENTER);
	    Font font = c.isAbstract() && !c.isInterface() ? Font.CLASS_ABSTRACT : Font.CLASS;
	    String qualifiedName = qualifiedName(opt, r);
	    int startTemplate = qualifiedName.indexOf('<');
	    int idx = 0;
	    if(startTemplate < 0)
		idx = qualifiedName.lastIndexOf('.');
	    else
		idx = qualifiedName.lastIndexOf('.', startTemplate);
	    if(opt.postfixPackage && idx > 0 && idx < (qualifiedName.length() - 1)) {
		String packageName = qualifiedName.substring(0, idx);
		String cn = className.substring(idx + 1);
		tableLine(Align.CENTER, escape(cn), opt, font);
		tableLine(Align.CENTER, packageName, opt, Font.PACKAGE);
	    } else {
		tableLine(Align.CENTER, escape(qualifiedName), opt, font);
	    }
	    tagvalue(opt, c);
	    innerTableEnd();
	    
	    boolean showMembers =
		(opt.showAttributes && c.fields().length > 0) ||
		(c.isEnum() && opt.showEnumConstants && c.enumConstants().length > 0) ||
		(opt.showOperations && c.methods().length > 0) ||
		(opt.showConstructors && c.constructors().length > 0);
	    
	    if (showMembers) {
		if (opt.showAttributes) {
		    innerTableStart();
		    FieldDoc[] fields = c.fields();
		    // if there are no fields, print an empty line to generate proper HTML
		    if (fields.length == 0)
			tableLine(Align.LEFT, "");
		    else
			attributes(opt, c.fields());
		    innerTableEnd();
		} else if(!c.isEnum() && (opt.showConstructors || opt.showOperations)) {
		    // show an emtpy box if we don't show attributes but
		    // we show operations
		    innerTableStart();
		    tableLine(Align.LEFT, "");
		    innerTableEnd();
	    	}
		if (c.isEnum() && opt.showEnumConstants) {
		    innerTableStart();
		    FieldDoc[] ecs = c.enumConstants();
		    // if there are no constants, print an empty line to generate proper HTML		    
		    if (ecs.length == 0) {
			tableLine(Align.LEFT, "");
		    } else {
			for (FieldDoc fd : c.enumConstants()) {
			    tableLine(Align.LEFT, fd.name());
			}
		    }
		    innerTableEnd();
		}
		if (!c.isEnum() && (opt.showConstructors || opt.showOperations)) {
		    innerTableStart();
		    boolean printedLines = false;
		    if (opt.showConstructors)
			printedLines |= operations(opt, c.constructors());
		    if (opt.showOperations)
			printedLines |= operations(opt, c.methods());

		    if (!printedLines)
			// if there are no operations nor constructors,
			// print an empty line to generate proper HTML
			tableLine(Align.LEFT, "");

		    innerTableEnd();
		}
	    }
	    externalTableEnd();
	    nodeProperties(opt);
	    ci.nodePrinted = true;
	}
	return ci.name;
    }

    private String getNodeName(ClassDoc c) {
	String className = c.toString();
	ClassInfo ci = getClassInfo(className);
	if (ci == null)
	    ci = newClassInfo(className, false, hidden(c));
	return ci.name;
    }
    
    /** Return a class's internal name */
    private String getNodeName(String c) {
	ClassInfo ci = getClassInfo(c);

	if (ci == null)
	    ci = newClassInfo(c, false, false);
	return ci.name;
    }

    /**
     * Print all relations for a given's class's tag
     * @param tagname the tag containing the given relation
     * @param from the source class
     * @param fromName the source class internal name
     * @param edgetype the dot edge specification
     */
    private void relation(Options opt, RelationType rt, ClassDoc from, String fromName) {
	String tagname = rt.toString().toLowerCase();
	for (Tag tag : from.tags(tagname)) {
	    String t[] = StringUtil.tokenize(tag.text());    // l-src label l-dst target
	    if (t.length != 4) {
		System.err.println("Error in " + from + "\n" + tagname + " expects four fields (l-src label l-dst target): " + tag.text());
		return;
	    }
	    String dest = t[3];
	    String destName = null;
	    ClassDoc to = from.findClass(t[3]);
	    if(to != null) {
		dest = to.toString();
		destName = getNodeName(dest);
	    } else {
		destName = getNodeName(t[3]);
	    }
	    
	    if(hidden(dest))
		continue;
	    relation(opt, rt, from.toString(), fromName, dest, destName, t[0], t[1], t[2]);
	}
    }

    private void relation(Options opt, RelationType rt, String from, String fromName, 
	    String dest, String destName, String tailLabel, String label, String headLabel) {
	// print relation
	String edgetype = associationMap.get(rt);
	w.println("\t// " + from + " " + rt.toString() + " " + dest);
	w.println("\t" + fromName + ":p -> " + destName + ":p [" +
    	"taillabel=\"" + tailLabel + "\", " +
    	"label=\"" + guillemize(opt, label) + "\", " +
    	"headlabel=\"" + headLabel + "\", " +
    	"fontname=\"" + opt.edgeFontName + "\", " +
    	"fontcolor=\"" + opt.edgeFontColor + "\", " +
    	"fontsize=" + opt.edgeFontSize + ", " +
    	"color=\"" + opt.edgeColor + "\", " +
    	edgetype + "];"
    	);
	
	// update relation info
	RelationDirection d = RelationDirection.BOTH;
	if(rt == RelationType.NAVASSOC || rt == RelationType.DEPEND)
	    d = RelationDirection.OUT;
	getClassInfo(from).addRelation(dest, rt, d);
        getClassInfo(dest).addRelation(from, rt, d.inverse());
	
    }

    /** Print a class's relations */
    public void printRelations(ClassDoc c) {
	Options opt = optionProvider.getOptionsFor(c);
	if (hidden(c) || c.name().equals("")) // avoid phantom classes, they may pop up when the source uses annotations
	    return;
	String className = c.toString();
	String cs = getNodeName(c);

	// Print generalization (through the Java superclass)
	Type s = c.superclassType();
	if (s != null &&
	    !s.toString().equals("java.lang.Object") &&
	    !c.isEnum() &&
	    !hidden(s.asClassDoc())) {
	    	ClassDoc sc = s.asClassDoc();
		w.println("\t//" + c + " extends " + s + "\n" +
		    "\t" + getNodeName(sc) + ":p -> " + cs + ":p [dir=back,arrowtail=empty];");
		getClassInfo(className).addRelation(sc.toString(), RelationType.EXTENDS, RelationDirection.OUT);
		getClassInfo(sc.toString()).addRelation(className, RelationType.EXTENDS, RelationDirection.IN);
	}

	// Print generalizations (through @extends tags)
	for (Tag tag : c.tags("extends"))
	    if (!hidden(tag.text())) {
		w.println("\t//" + c + " extends " + tag.text() + "\n" +
		    "\t" + getNodeName(tag.text()) + ":p -> " + cs + ":p [dir=back,arrowtail=empty];");
		getClassInfo(className).addRelation(tag.text(), RelationType.EXTENDS, RelationDirection.OUT);
		getClassInfo(tag.text()).addRelation(className, RelationType.EXTENDS, RelationDirection.IN);
	    }
	// Print realizations (Java interfaces)
	for (Type iface : c.interfaceTypes()) {
	    ClassDoc ic = iface.asClassDoc();
	    if (!hidden(ic)) {
		w.println("\t//" + c + " implements " + ic + "\n\t" + getNodeName(ic) + ":p -> " + cs + ":p [dir=back,arrowtail=empty,style=dashed];"
		    );
		getClassInfo(className).addRelation(ic.toString(), RelationType.IMPLEMENTS, RelationDirection.OUT);
		getClassInfo(ic.toString()).addRelation(className, RelationType.IMPLEMENTS, RelationDirection.IN);
	    }
	}
	// Print other associations
	relation(opt, RelationType.ASSOC, c, cs);
	relation(opt, RelationType.NAVASSOC, c, cs);
	relation(opt, RelationType.HAS, c, cs);
	relation(opt, RelationType.COMPOSED, c, cs);
	relation(opt, RelationType.DEPEND, c, cs);
    }

    /** Print classes that were parts of relationships, but not parsed by javadoc */
    public void printExtraClasses(RootDoc root) {
	Set<String> names = new HashSet<String>(classnames.keySet()); 
	for(String className: names) {
	    ClassInfo info = getClassInfo(className);
	    if (!info.nodePrinted) {
		ClassDoc c = root.classNamed(className);
		if(c != null) {
		    printClass(c, false);
		} else {
		    Options opt = optionProvider.getOptionsFor(className);
		    if(opt.matchesHideExpression(className))
			continue;
		    w.println("\t// " + className);
		    w.print("\t" + info.name + "[label=");
		    externalTableStart(opt, className, classToUrl(className));
		    innerTableStart();
		    int idx = className.lastIndexOf(".");
		    if(opt.postfixPackage && idx > 0 && idx < (className.length() - 1)) {
			String packageName = className.substring(0, idx);
			String cn = className.substring(idx + 1);
			tableLine(Align.CENTER, escape(cn), opt, Font.CLASS);
			tableLine(Align.CENTER, packageName, opt, Font.PACKAGE);
		    } else {
			tableLine(Align.CENTER, escape(className), opt, Font.CLASS);
		    }
		    innerTableEnd();
		    externalTableEnd();
		    nodeProperties(opt);
		}
	    }
	}
    }
    
    /**
     * Prints associations recovered from the fields of a class. An association is inferred only
     * if another relation between the two classes is not already in the graph.
     * @param classes
     */    
    public void printInferredRelations(ClassDoc[] classes) {
        for (ClassDoc c : classes) {
            printInferredRelations(c);
        }
    }
    
    /**
     * Prints associations recovered from the fields of a class. An association is inferred only
     * if another relation between the two classes is not already in the graph.
     * @param classes
     */  
    public void printInferredRelations(ClassDoc c) {
	Options opt = optionProvider.getOptionsFor(c);

	// check if the source is excluded from inference
	String sourceName = c.toString();
	if (hidden(c))
	    return;

	for (FieldDoc field : c.fields(false)) {
	    // skip statics
	    if(field.isStatic())
		continue;
	    
	    // skip primitives
	    FieldRelationInfo fri = getFieldRelationInfo(field);
	    if (fri == null)
		continue;

	    // check if the destination is excluded from inference
	    if (hidden(fri.cd))
		continue;

	    String dest = fri.cd.toString();
	    String destAdornment = fri.multiple ? "*" : "";
	    relation(opt, opt.inferRelationshipType, sourceName, getNodeName(c), dest,
		    getNodeName(dest), "", "", destAdornment);
	}
    }

    /**
     * Prints dependencies recovered from the methods of a class. A
     * dependency is inferred only if another relation between the two
     * classes is not already in the graph.
     * @param classes
     */    
    public void printInferredDependencies(ClassDoc[] classes) {
	for (ClassDoc c : classes) {
	    printInferredDependencies(c);
	}
    }
    
    /**
     * Prints dependencies recovered from the methods of a class. A
     * dependency is inferred only if another relation between the two
     * classes is not already in the graph.
     * @param classes
     */  
    public void printInferredDependencies(ClassDoc c) {
	Options opt = optionProvider.getOptionsFor(c);

	String sourceName = c.toString();
	if (hidden(c))
	    return;

	Set<Type> types = new HashSet<Type>();
	// harvest method return and parameter types
	for (MethodDoc method : filterByVisibility(c.methods(false), opt.inferDependencyVisibility)) {
	    types.add(method.returnType());
	    for (Parameter parameter : method.parameters()) {
		types.add(parameter.type());
	    }
	}
	// and the field types
	if (!opt.inferRelationships) {
	    for (FieldDoc field : filterByVisibility(c.fields(false), opt.inferDependencyVisibility)) {
		types.add(field.type());
	    }
	}
	// see if there are some type parameters
	if (c.asParameterizedType() != null) {
	    ParameterizedType pt = c.asParameterizedType();
	    types.addAll(Arrays.asList(pt.typeArguments()));
	}
	// see if type parameters extend something
	for(TypeVariable tv: c.typeParameters()) {
	    if(tv.bounds().length > 0 )
		types.addAll(Arrays.asList(tv.bounds()));
	}
	
	// and finally check for explicitly imported classes (this
	// assumes there are no unused imports...)
	if (opt.useImports)
	    types.addAll(Arrays.asList(c.importedClasses()));

	// compute dependencies
	for (Type type : types) {
	    // skip primitives and type variables, as well as dependencies
	    // on the source class
	    if (type.isPrimitive() || type instanceof WildcardType || type instanceof TypeVariable
		    || c.toString().equals(type.asClassDoc().toString()))
		continue;

	    // check if the destination is excluded from inference
	    ClassDoc fc = type.asClassDoc();
	    String destName = fc.toString();
	    if (hidden(fc))
		continue;
	    
	    // check if source and destination are in the same package and if we are allowed
	    // to infer dependencies between classes in the same package
	    if(!opt.inferDepInPackage && c.containingPackage().equals(fc.containingPackage()))
		continue;

	    // if source and dest are not already linked, add a dependency
	    RelationPattern rp = getClassInfo(sourceName).getRelation(destName);
	    if (rp == null || rp.matchesOne(new RelationPattern(RelationDirection.OUT))) {
		relation(opt, RelationType.DEPEND, sourceName, getNodeName(sourceName), destName,
			getNodeName(destName), "", "", "");
	    }
	    
	}
    }
    
    /**
     * Returns all program element docs that have a visibility greater or
     * equal than the specified level
     */
    private <T extends ProgramElementDoc> List<T> filterByVisibility(T[] docs, Visibility visibility) {
	if (visibility == Visibility.PRIVATE)
	    return Arrays.asList(docs);

	List<T> filtered = new ArrayList<T>();
	for (T doc : docs) {
	    if (Visibility.get(doc).compareTo(visibility) > 0)
		filtered.add(doc);
	}
	return filtered;
    }



    private FieldRelationInfo getFieldRelationInfo(FieldDoc field) {
	Type type = field.type();
	if(type.isPrimitive() || type instanceof WildcardType || type instanceof TypeVariable)
	    return null;
	
	if (type.dimension().endsWith("[]")) {
	    return new FieldRelationInfo(type.asClassDoc(), true);
	}
	
	Options opt = optionProvider.getOptionsFor(type.asClassDoc());
	if (opt.matchesCollPackageExpression(type.qualifiedTypeName())) {
	    Type[] argTypes = getInterfaceTypeArguments(collectionClassDoc, type);
	    if (argTypes != null && argTypes.length == 1) {
		return new FieldRelationInfo(argTypes[0].asClassDoc(), true);
	    }
	    argTypes = getInterfaceTypeArguments(mapClassDoc, type);
	    if (argTypes != null && argTypes.length == 2) {
		return new FieldRelationInfo(argTypes[1].asClassDoc(), true);
	    }
	}

	return new FieldRelationInfo(type.asClassDoc(), false);
    }
    
    private Type[] getInterfaceTypeArguments(ClassDoc iface, Type t) {
	if (t instanceof ParameterizedType) {
	    ParameterizedType pt = (ParameterizedType) t;
	    if (iface.equals(t.asClassDoc())) {
		return pt.typeArguments();
	    } else {
		for (Type pti : pt.interfaceTypes()) {
		    Type[] result = getInterfaceTypeArguments(iface, pti);
		    if (result != null)
			return result;
		}
		if (pt.superclassType() != null)
		    return getInterfaceTypeArguments(iface, pt.superclassType());
	    }
	} else if (t instanceof ClassDoc) {
	    ClassDoc cd = (ClassDoc) t;
	    for (Type pti : cd.interfaceTypes()) {
		Type[] result = getInterfaceTypeArguments(iface, pti);
		if (result != null)
		    return result;
	    }
	    if (cd.superclassType() != null)
		return getInterfaceTypeArguments(iface, cd.superclassType());
	}
	return null;
    }

    /** Removes the template specs from a class name. */
    private String removeTemplate(String name) {
	int openIdx = name.indexOf('<');
	if(openIdx == -1)
	    return name;
	else
	    return name.substring(0, openIdx);
    }

    /** Convert the class name into a corresponding URL */
    public String classToUrl(ClassDoc cd, boolean rootClass) {
	// building relative path for context and package diagrams
	if(contextDoc != null && rootClass) {
	    // determine the context path, relative to the root
	    String packageName = null;
	    if (contextDoc instanceof ClassDoc) {
		    packageName = ((ClassDoc) contextDoc).containingPackage().name();
		} else if (contextDoc instanceof PackageDoc) {
		    packageName = ((PackageDoc) contextDoc).name();
		} else {
		    return classToUrl(cd.qualifiedName());
		}
	    return buildRelativePath(packageName, cd.containingPackage().name()) + cd.name() + ".html";
	} else {
	    return classToUrl(cd.qualifiedName());
	} 
    }

    protected static String buildRelativePath(String contextPackageName, String classPackageName) {
	// path, relative to the root, of the destination class
	String[] contextClassPath = contextPackageName.split("\\.");
	String[] currClassPath = classPackageName.split("\\.");

	// compute relative path between the context and the destination
	// ... first, compute common part
	int i = 0;
	while (i < contextClassPath.length && i < currClassPath.length
		&& contextClassPath[i].equals(currClassPath[i]))
	    i++;
	// ... go up with ".." to reach the common root
	StringBuffer buf = new StringBuffer();
	if (i == contextClassPath.length) {
	    buf.append(".").append(FILE_SEPARATOR);
	} else {
	    for (int j = i; j < contextClassPath.length; j++) {
		buf.append("..").append(FILE_SEPARATOR);
	    }
	}
	// ... go down from the common root to the destination
	for (int j = i; j < currClassPath.length; j++) {
	    buf.append(currClassPath[j]).append(FILE_SEPARATOR);
	}
	return buf.toString();
    }
    
    /** Convert the class name into a corresponding URL */
    public String classToUrl(String className) {
        String docRoot = mapApiDocRoot(className);
        if (docRoot != null) {
            StringBuffer buf = new StringBuffer(docRoot);
            buf.append(className.replace('.', FILE_SEPARATOR));
            buf.append(".html");
            return buf.toString();
        } else {
            return null;
        }
    }

    /**
     * Returns the appropriate URL "root" for a given class name.
     * The root will be used as the prefix of the URL used to link the class in
     * the final diagram to the associated JavaDoc page.
     */
    private String mapApiDocRoot(String className) {
	String root = null;
	/* If no packages are specified, we use apiDocRoot for all of them. */
	if (rootClasses.contains(className)) {
	    root = optionProvider.getGlobalOptions().apiDocRoot;
	} else {
	    Options globalOptions = optionProvider.getGlobalOptions();
	    root = globalOptions.getApiDocRoot(className);
	}
	return root;
    }

    
    /** Dot prologue 
     * @throws IOException */
    public void prologue() throws IOException {
	Options opt = optionProvider.getGlobalOptions();
	// prepare output file. Use the output file name as a full path unless the output
	// directory is specified
	File file = null;
	if (opt.outputDirectory != null)
	    file = new File(opt.outputDirectory, opt.outputFileName);
	else
	    file = new File(opt.outputFileName);
	// make sure the output directory are there, otherwise create them
	if (file.getParentFile() != null
	    && !file.getParentFile().exists())
	    file.getParentFile().mkdirs();
	FileOutputStream fos = new FileOutputStream(file);
	
	// print plologue
	w = new PrintWriter(new OutputStreamWriter(new BufferedOutputStream(fos), opt.outputEncoding));
	w.println(
	    "#!/usr/local/bin/dot\n" +
	    "#\n" +
	    "# Class diagram \n" +
	    "# Generated by UmlGraph version " +
	    Version.VERSION + " (http://www.spinellis.gr/sw/umlgraph)\n" +
	    "#\n\n" +
	    "digraph G {\n" +
	    "\tedge [fontname=\"" + opt.edgeFontName +
	    "\",fontsize=10,labelfontname=\"" + opt.edgeFontName +
	    "\",labelfontsize=10];\n" +
	    "\tnode [fontname=\"" + opt.nodeFontName +
	    "\",fontsize=10,shape=plaintext];"
	);
	if (opt.horizontal)
	    w.println("\trankdir=LR;\n\tranksep=1;");
	if (opt.bgColor != null)
	    w.println("\tbgcolor=\"" + opt.bgColor + "\";\n");
    }

    /** Dot epilogue */
    public void epilogue() {
	w.println("}\n");
	w.flush();
	w.close();
    }
    
    private void externalTableStart(Options opt, String name, String url) {
	String bgcolor = "";
	if (opt.nodeFillColor != null)
	    bgcolor = " bgcolor=\""+ opt.nodeFillColor + "\"";
	String href = "";
	if (url != null)
	    href = " href=\"" + url + "\"";
	w.print("<<table border=\"0\" cellborder=\"1\" cellspacing=\"0\" "
		+ "cellpadding=\"2\" port=\"p\"" + bgcolor + href + ">" + linePostfix);
    }
    
    private void externalTableEnd() {
	w.print(linePrefix + linePrefix + "</table>>");
    }
    
    private void innerTableStart() {
	w.print(linePrefix + linePrefix + "<tr><td><table border=\"0\" cellspacing=\"0\" "
		+ "cellpadding=\"1\">" + linePostfix);
    }
    
    private void innerTableEnd() {
	w.print(linePrefix + linePrefix + "</table></td></tr>" + linePostfix);
    }
    
    private void tableLine(Align align, String text) {
	tableLine(align, text, null, Font.NORMAL);
    }
    
    private void tableLine(Align align, String text, Options opt, Font font) {
	String open;
	String close = "</td></tr>";
	String prefix = linePrefix + linePrefix + linePrefix;
	if(align == Align.CENTER)
	    open = prefix + "<tr><td>"; 
	else if(align == Align.LEFT)
	    open = prefix + "<tr><td align=\"left\">";
	else if(align == Align.RIGHT)
	    open = prefix + "<tr><td align=\"right\">";
	else
	    throw new RuntimeException("Unknown alignement type " + align);
	
	text = fontWrap(" " + text + " ", opt, font);
	w.print(open + text + close + linePostfix);
    }
    
    /**
     * Wraps the text with the appropriate font according to the specified font type
     * @param opt
     * @param text
     * @param font
     * @return
     */
    private String fontWrap(String text, Options opt, Font font) {
	if(font == Font.ABSTRACT) { 
	    return fontWrap(text, opt.nodeFontAbstractName, opt.nodeFontSize);
	} else if(font == Font.CLASS) {
	    return fontWrap(text, opt.nodeFontClassName, opt.nodeFontClassSize);
	} else if(font == Font.CLASS_ABSTRACT) {
	    String name;
	    if(opt.nodeFontClassAbstractName == null)
		name = opt.nodeFontAbstractName;
	    else
		name = opt.nodeFontClassAbstractName;
	    return fontWrap(text, name, opt.nodeFontClassSize);
	} else if(font == Font.PACKAGE) {
	    return fontWrap(text, opt.nodeFontPackageName, opt.nodeFontPackageSize);
	} else if(font == Font.TAG) {
	    return fontWrap(text, opt.nodeFontTagName, opt.nodeFontTagSize);
	} else {
	    return text;
	}
    }
    
    /**
     * Wraps the text with the appropriate font tags when the font name
     * and size are not void
     * @param text the text to be wrapped
     * @param fontName considered void when it's null 
     * @param fontSize considered void when it's <= 0
     */
    private String fontWrap(String text, String fontName, double fontSize) {
	if(fontName == null && fontSize == -1)
	    return text;
	else if(fontName == null)
	    return "<font point-size=\"" + fontSize + "\">" + text + "</font>";
	else if(fontSize <= 0)
	    return "<font face=\"" + fontName + "\">" + text + "</font>";
	else
	    return "<font face=\"" + fontName + "\" point-size=\"" + fontSize + "\">" + text + "</font>";
    }
    
    private static class FieldRelationInfo {
	ClassDoc cd;
	boolean multiple;

	public FieldRelationInfo(ClassDoc cd, boolean multiple) {
	    this.cd = cd;
	    this.multiple = multiple;
	}
    }
}
