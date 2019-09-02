# ELM Customers Technical Task


## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

What things you need to install the software and how to install them

```
Change databse connections string in appsettings.json to match your server and restore ELMCustomers.bak file.
```

```
Make sure rabbitmq is up and running on port 15672
```

## Running the project

Navigate to each of the following project folder execute this command

```
dotnet run
```

Folders

```
/ELM.Customers.Consumer
```
```
/ELM.Notifications.Consumer
```
```
/ELM.Customers.API
```
```
/ELM.Notifications.API
```

### Testing the project

```
Open customers API and navigate to swagger, Then execute Patch request for the cutomers API.
```


## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Thanks

Islam Abdelrazik