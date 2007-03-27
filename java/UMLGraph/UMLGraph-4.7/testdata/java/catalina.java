/*
 * Interface and generalization relationships in Jakarta Catalina
 * $Id: catalina.java,v 1.1 2005/11/23 22:18:45 dds Exp $
 */

class HttpResponseBase
	extends ResponseBase
	implements HttpResponse, HttpServletResponse {}

abstract class HttpResponseWrapper
	extends ResponseWrapper 
	implements HttpResponse {}

class HttpResponseFacade
	extends ResponseFacade 
	implements HttpServletResponse {}

abstract class ResponseWrapper implements Response {}
abstract interface HttpResponse extends Response {}
abstract class ResponseBase implements Response, ServletResponse {}
abstract interface HttpServletResponse {}
class ResponseFacade implements ServletResponse {}

abstract interface ServletResponse {}
abstract interface Response {}
