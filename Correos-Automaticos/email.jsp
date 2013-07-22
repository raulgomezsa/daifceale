<%@ page import="java.io.*" %>
<%@ page import="java.util.*" %>
<%@ page import="java.util.Date" %>
<%@ page import="java.text.SimpleDateFormat" %>
<%@ page import="java.sql.*" %>
<%@ page import = "javax.mail.internet.*" %>
<%@ page import = "javax.mail.*" %>
<%@ page import = "javax.net.*" %>

<%! 
	String  ipDB, portDB, nameDB, userDB, passDB;
	String nombre, apellido, nombre2, apellido2, consulta, cadena, cadena2, usermail, usermail2;
	String tiempo;
	ResourceBundle rb;
	Connection conexion;
	Statement st;
	ResultSet rs;
	String ruta_dialogos;
%>
<html>
<head> 
	<title>Email</title>
</head>
<body>
<%

	rb = ResourceBundle.getBundle("config");
	
	// Leemos los datos necesarios para la conexión con la base de datos

	ipDB = rb.getString("ip_abant");
	portDB = rb.getString("port_abant");
	nameDB = rb.getString("db_robust");
	userDB = rb.getString("user_abant");
	passDB = rb.getString("password_abant");
	ruta_dialogos = new String("C:/Servidor/Dialogos/");

	try {
        Class.forName("com.mysql.jdbc.Driver").newInstance();
        conexion = DriverManager.getConnection("jdbc:mysql://"+ipDB+":"+portDB+"/"+nameDB,userDB,passDB);
		if (!conexion.isClosed())
		{
			out.println("Conectado. <br>");
			// Solicitamos los datos que necesitamos.
			cadena = request.getParameter("nombre");
			cadena2 = request.getParameter("nombre2");
			tiempo = request.getParameter("tiempo");
			tiempo = String.valueOf(Integer.parseInt(tiempo)+20);

			//Fecha y hora de fin de la actividad
			Date fechaActual=new Date();
			SimpleDateFormat formato = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
			String cadenaFecha = formato.format(fechaActual);

			//Fecha y hora de inicio de la actividad
			Calendar cal = new GregorianCalendar();
			cal.setTimeInMillis(fechaActual.getTime());
			cal.add(Calendar.SECOND, -Integer.decode(tiempo));
			Date fechaInicio=new Date(cal.getTimeInMillis());
			String cadenaFecha2 = formato.format(fechaInicio);

			//out.println(cadenaFecha+"<br>"+cadenaFecha2+"<br>");
			
			// Comprobamos que los parámetros no estén vacíos.
			if (cadena!= null && cadena2!= null)
			{
				// Separamos el nombre y el apellido en dos variables.
				
				String[] array_cadena = cadena.split(" ");
				String[] array_cadena2 = cadena2.split(" ");
				Integer nfichero = 0;
				nombre = array_cadena[0];
				apellido = array_cadena[1];
				nombre2 = array_cadena2[0];
				apellido2 = array_cadena2[1];
				
				// Generamos la consulta.
				consulta = "select Email from useraccounts where FirstName='"+nombre+"' && LastName='"+apellido+"';";
				
				// Ejecutamos la consulta.
				st = conexion.createStatement();
				rs = st.executeQuery(consulta);
				
				// Leemos el dato que queremos del resultado de la consulta.
				if (rs.first() == true)
				{
					usermail = rs.getString("Email");
				}
				
				consulta = "select Email from useraccounts where FirstName='"+nombre2+"' && LastName='"+apellido2+"';";
				
				// Ejecutamos la consulta.
				st = conexion.createStatement();
				rs = st.executeQuery(consulta);
				
				// Leemos el dato que queremos del resultado de la consulta.
				if (rs.first() == true)
				{
					usermail2 = rs.getString("Email");
				}
				
				//Comprobamos si el fichero en el que queremos introducir los datos de salida existe.
				//Si no existe lo creamos con la consulta de MySQL que hay unas líneas mas abajo.
				File fichero=new File (ruta_dialogos+"dialogo"+ String.valueOf(nfichero) +".xls");
				while(fichero.exists())
				{
					++nfichero;
					fichero=new File (ruta_dialogos+"dialogo"+ String.valueOf(nfichero) +".xls");
				}

				String nombre_archivodestino = ruta_dialogos+"dialogo"+ String.valueOf(nfichero) +".xls";
				
				// Generamos la consulta.
				consulta = "SELECT _datetime, FirstName, LastName, `_message` "+
				"FROM `_bournechannel`, useraccounts "+
				"WHERE `_speaker`=PrincipalID AND ( (FirstName ='"+nombre+"' AND LastName='"+apellido+"') OR (FirstName='"+nombre2+"' AND LastName='"+apellido2+"'))" +
				"AND (_datetime BETWEEN '"+cadenaFecha2+"' AND '"+cadenaFecha+"')"+
				"INTO OUTFILE '"+nombre_archivodestino+"';";

				// Ejecutamos la consulta.
				st = conexion.createStatement();
				rs = st.executeQuery(consulta);

	//Abrimos dos ficheros. En file introducimos el xls generado en la consulta sql, que contiene todo lo
	// relacionado con el dialogo de los alumnos (fecha, hora, nombre, apellido, mensaje) separados por espacios.
	//En el segundo, file2, tenemos el nombre de los alumnos, cuyo avatar tenga de nombre "Alumno numero", que 
	//necesitamos convertir en nombre.

	String alumno1= new String();
	String alumno2= new String();
	
	if(nombre.equals("Alumno") || nombre2.equals("Alumno"))
	{
		File file2 = new File("C:/Servidor/Dialogos/Nombres/Alumnos.txt");

		FileInputStream fisAlum = null;
		BufferedInputStream bisAlum = null;
		DataInputStream disAlum = null;
		
		fisAlum = new FileInputStream(file2);
		bisAlum = new BufferedInputStream(fisAlum);
		disAlum = new DataInputStream(bisAlum);

		String temp=new String();
		while (disAlum.available() != 0) {
			temp=disAlum.readLine();
			String[] alumnos = temp.split(" ");
			if(apellido.equals(alumnos[0]))
			{
				alumno1=alumnos[1];
			}
			else if(apellido2.equals(alumnos[0]))
			{
				alumno2=alumnos[1];
			}
		}
		fisAlum.close();
		bisAlum.close();
		disAlum.close();
	}


	File file = new File(nombre_archivodestino);
	
	FileInputStream fis = null;
	BufferedInputStream bis = null;
	DataInputStream dis = null;
	
	fis = new FileInputStream(file);
	bis = new BufferedInputStream(fis);
	dis = new DataInputStream(bis);
	
	String dialogo=new String();
	
	while (dis.available() != 0) {
		String temp = dis.readLine();
		String[] vector_dia= temp.split("\\s+");
		if(vector_dia[2].equals("Alumno"))
		{
			if(vector_dia[3].equals(apellido))
			{
				vector_dia[2]=alumno1;
				vector_dia[3]=" ";
			}
			else if(vector_dia[3].equals(apellido2))
			{
				vector_dia[2]=alumno2;
				vector_dia[3]=" ";
			}
			int size = vector_dia.length;
			String temp2=new String();
			for (int i=0; i<size; i++)
			{
				temp2+=vector_dia[i]+" ";
			}
			temp=temp2;
		}

		if(!nombre.equals("Alumno"))
		{
			alumno1=nombre;
		}
		if(!nombre2.equals("Alumno"))
		{
			alumno2=nombre2;
		}
		
		if(temp.indexOf(alumno1.toString())!=-1  )
		{
			dialogo+="<div style='color:green;'>"+temp+"</div>";
		}
		else
		{
			dialogo+="<div style='color:red;'>"+temp+"</div>";
		}
	}

	// dispose all the resources after using them.
	fis.close();
	bis.close();
	dis.close();

	//out.println(dialogo+"<br>");
				
				
				// Ya disponemos de la dirección email del usuario, procedemos a enviarle el correo.
				Session sesion;
				MimeMessage message;
				Transport t;

				//Debemos introducir una cuenta de correo válida desde la que se va a enviar el correo (se recomienda Gmail)
				String servermaildir = //cuenta de correo;
				String servermailpass = //contraseña de la cuenta de correo;

				Properties props = new Properties();
				props.setProperty("mail.smtp.host", "smtp.gmail.com");
				props.setProperty("mail.smtp.starttls.enable", "true");
				props.setProperty("mail.smtp.port","587");
				props.setProperty("mail.smtp.user", usermail);
				props.setProperty("mail.smtp.auth", "true");

				sesion = Session.getDefaultInstance(props);
				sesion.setDebug(false);

                try 
				{
					// Composicion de la comunicacion del mensaje.

                    message = new MimeMessage(sesion);
                    message.setFrom(new InternetAddress(servermaildir));
                    message.addRecipient(Message.RecipientType.TO, new InternetAddress(usermail));
                    message.addRecipient(Message.RecipientType.TO, new InternetAddress(usermail2));
                    message.setSubject("Herzlichen Glückwunsch! - Anke");
                    message.setText("<html>"+
									"<head><title>Herzlichen Glückwunsch!</title></head>"+
									"<body bgcolor=#f2edc8><div style='margin-top:10px;'><center>"+
									"<table border=0 width='100%'><tr>"+
									"<td align=center><img src=http://www2.uca.es/serv/rel_institucionales/album/Logos%20UCA/logo1a.gif></td></td>"+
									"</tr></table>"+
									"<h1 style='color:green;'>Hallo "+ alumno1 + " und " + alumno2 + "!</h1><br>"+
									"<h1 style='color:blue;'>Vielen, vielen Dank für Euer Engagement und Eure Unterstützung!</h1><br>"+
									"<h3 style='color:grey;'>Hier eine Kopie Eures text-chats:<br><br>"+
									"</h3></center></div><div style='margin-top:10px;'><left><p>"+  dialogo +
									"</p></left></div></body></html>",
                                    "ISO-8859-1",
                                    "html");

                    t = sesion.getTransport("smtp");
                    t.connect(servermaildir,servermailpass);
                    t.sendMessage(message,message.getAllRecipients());
					
					out.println("Mensaje enviado correctamente.");
					
                    t.close();
				}
				catch(MessagingException mex)
				{
					out.println("Fallo en el envio: " + mex);
				}
	
				// Cerramos ResulSet y Statement
				rs.close();
				st.close();
			}
			else
			{
			out.println("No se recibieron parámemtros.");
			}
			
			// Cerramos Connection.
			conexion.close();
		}
		else
		{
			out.println("Error de conexión.");
		}
    }
    catch (Exception e) {
        e.printStackTrace();
    }
	
%>
</body>
</html>
