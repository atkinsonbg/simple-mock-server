# Mock-Server
The goal of this project is to provide a simple mock server for mocking external API calls. Mock-Server runs as a standalone Docker container, which you can call via your code in order to mock API calls. When you run the container you mount a folder, "Mocks", from your local environment to the container. When the container starts, it loads all these mocks into memory and will return the appropriate data for your call. A mock file looks like the following:

```
[
    {
        "description": "Internal mock-server test to make sure the routing works.",
        "url": "/mockservertest/mock1",
        "method": "GET",
        "response": {
            "statuscode": 200,
            "headers": [
                {
                    "accept": "application/json"
                },
                {
                    "custom": "headertype"
                },
                {
                    "content-type": "application/json"
                }
            ],
            "body": {
                "method": "GET",
                "route": "mock2"
            }
        }
    },
    {
        "description": "Internal mock-server a test to make sure the routing works.",
        "url": "/mockservertest/mock1",
        "method": "POST",
        "response": {
            "statuscode": 200,
            "headers": [
                {
                    "accept": "application/json"
                },
                {
                    "custom": "headertype"
                },
                {
                    "content-type": "application/json"
                }
            ],
            "body": {
                "method": "POST",
                "route": "mock1"
            }
        }
    },
    {
        "description": "This is a test to make sure the routing works.",
        "url": "/mockservertest/mock2",
        "method": "GET",
        "response": {
            "statuscode": 200,
            "headers": [
                {
                    "accept": "application/json"
                },
                {
                    "custom": "headertype"
                },
                {
                    "content-type": "application/json"
                }
            ],
            "body": {
                "method": "GET",
                "route": "mock2"
            }
        }
    }
]
```
This file would be saved with any name you like, but must have a `.json` file extension, for instance: `./Mocks/mocksertest.json`. When the container starts it simply looks for any `.json` files in its `Mocks` folder and loads them into memory.


