# remote-environment-manager

This application is used to organise and control developing environment on remote work machines.<br>
Work machines are sorted in rooms/offices. <br> 
Server can execute ansible playbooks to one or multiple machines, room/office or multiple rooms/offices or combination of both. <br>
Server can:<ul> 
<li>build docker image</li>
<li>run, stop and delete docker containers</li>
<li>upload files to server</li>
<li>push and pull files to machines</li>
</ul>
For all those functionalities user needs to be authenticated. <br>

## Explaining the flow
First user needs to login. <br>
Then user needs to create session. To create session user needs to specify some name that will later use for referencing in other API calls. After this user is free to do anything.<br>
Recommended path is to create inventory file with machines that it plans to use. This is later used as default inventory if subset name or machine list is not provided. <br>
User can also create inventory subset by providing session name, name for referencing and list of machines. <br>
If user doesn't plan on using any inventory file it can specify list of machines directly into command. <br>
Executing processes is async and user gets response as soons the process start.
User can get information about active processes on /status API. <br>
After finishing user must delete session. <br>
![plot](./flowchart.png)
# Use Case

1. Login with email and password on <br>
POST /Auth/login
2. Create session with session name on <br>
POST /Auth/session
3. (OPTIONAL) Add more users to session with email<br>
PUT /Auth/session?session={name}&email={email}
4. Get list of all machines or machines from specific room <br>
GET /Room/machines<br>
GET /Room/machines/{roomID}
5. Create inventory file with list of machines. This will be our default inventory and we don't need to specify those machines anymore. <br>
POST /Room/inventory?session={session_name}
6. Build docker image for wanted class by specifing image name and dockerfile name. <br>
 NOTE: Building image will be replaced with pulling image from docker registry to save up time.<br>
POST /build_image?session={session_name}&predmet={image_name}&image={Dockerfile}
7. Start docker container with now built image by specifing container name and image name <br>
POST /start_container?session={session_name}&predmet={image_name}&image={Dockerfile}
8. Upload files to server so we can push them. Added API helps maintain accessibility throuh all APIs.
POST /upload?session={session_name}
9. Push uploaded files to clients /home/machine/materijal.<br>
POST /push_files?session={session_name}
10. Pull files from clients /home/machine/rad.<br>
POST /pull_files?session={session_name}
11. Download pulled files in zipped format.<br>
GET /download?session={session_name}<br>
12. Destroy session. <br>
DELETE /Auth/session?session={session_name}<br>
<br>
<b>NOTE:</b> Status of any step from 6-10 can be seen on<br>
GET /status?session={session_name}<br>
![plot](./UseCaseDiagram.png)
## Examples of classes that can benefit containerization
<ul>
<li>Logičko i funkcijsko programiranje - preinstalled Haskell and Scala</li>
<li>Uvod u vestacku inteligenciju - python modules</li>
<li>Inteligentni sistemi - python modules</li>
<li>Web 1 - setup PHP and Mysql server</li>
<li>Web 2 - installed nodejs and npm</li>
...
</ul>