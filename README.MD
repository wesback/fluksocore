_____ _       _               ____               
|  ___| |_   _| | _____  ___  / ___|___  _ __ ___ 
| |_  | | | | | |/ / __|/ _ \| |   / _ \| '__/ _ \
|  _| | | |_| |   <\__ \ (_) | |__| (_) | | |  __/
|_|   |_|\__,_|_|\_\___/\___/ \____\___/|_|  \___|

### What is it?
This is some .NET Core code that will read the data from the MQTT feed coming from a Flukso device.
It will send the output to the console or Azure Event Hubs, feel free to extend the code wherever needed.

### Getting started
I have created this to run on a Raspberry PI using Docker.  You can find the deployment scripts for Azure in the Terraform folder.

#### Terraform
Go to the Terraform folder and execute the following
```
az login 
terraform init 
terraform plan -out=out.tfplan
terraform apply out.tfplan
```

#### docker-compose
You can find the sensor id in your user settings on https://www.flukso.net/.
Replace the values between {}.

```
version: '2.0'

services:
  fluksocore:
    image: wesback/fluksocore
    restart: always
    container_name: fluksocore
    environment:
      - FluksoServer:FluksoIP={ipaddress}
      - EventHubs:EHConnectionString=Endpoint=sb://{namespace}.servicebus.windows.net/;SharedAccessKeyName={keyname};SharedAccessKey={key}
      - EventHubs:EHEntityPath={ehpath} 
      - Sensors:Electricity={sensoride}
      - Sensors:Water={sensoridw}
      - Sensors:Gas={sensoridg}
      - Output:Console=false
      - Output:EventHubs=true
```

#### fluksocore.service
Now you can go ahead and add this as a service on your Raspberry PI.

```
cp fluksocore.service /etc/systemd/system
chmod 755 /etc/systemd/system/fluksocore.service
systemctl enable fluksocore.service
systemctl start fluksocore.service
systemctl status fluksocore.service
```

                                                   
