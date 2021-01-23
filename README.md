# Mock-Server
The goal of this project is to provide a simple mock server for mocking external API calls. Mock-Server runs as a standalone Docker container, which you can call via your code in order to mock API calls. When you run the container you mount a folder, "Mocks", from your local environment to the container. When the container starts, it loads all these mocks into memory and will return the appropriate data for your call. 

This is meant to be simple at its core. There are no fancy bells and whistles here: no recording and storing of requests/responses, no sharing of mocks across multiple repos, etc. Just mount of folder of mocks you care about and run it. It was designed so you could pull the Docker image and start testing, without the need for a lot of setup.

## Docker Image
A pre-built Docker image can be located here: https://hub.docker.com/repository/docker/atkinsonbg/mock-server


## How It Works
When you start the Mock Server container, it reads all the JSON files it can find in a root level directory located at `/api/Mocks`. It loads these into memory and uses them to respond to any requests the container receives. 

A mock file looks like the following, and must be an array of JSON regardless of how many mocks you have in the file:
```
[
    {
        "description": "Internal mock-server test to make sure the routing works.",
        "url": "/mockservertest/mock1",
        "method": "GET",
        "request": {
            "body": {
                "obj1": "value1",
                "obj2": "value2"
            },
            "headers": {
                "Host": "localhost:5000",
                "Referer": "http://localhost:5000/mockservertest/mock5",
                "Content-Length": "48",
                "custom2": "hello"
            }
        },
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
        "request": {
            "body": {
                "field1": "Hello",
                "field2": "World"
            },
            "headers": {
                "Host": "localhost:5000",
                "Referer": "http://localhost:5000/mockservertest/mock5",
                "Content-Length": "48",
                "custom1": "hello"
            }
        },
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
    }
]
```

As noted above, the structure of each JSON file is an array of mocks you want to load. This gives you a lot of flexibility in organzing your mocks. For instance, you could have any combination of the following:
- A single JSON file, with only one mock in each file
- A single JSON file, with multiple mocks in each file
- Subdirectorys with JSON files in them

This gives you flexibility in how you organize and store your mocks. The only requirement is they are all mounted to the `/api/Mocks` directory in the container.

## How Matching Works
When a request is sent to the mock container it is evaluated in the following order:
1. Match against the URL and METHOD.
2. If more than one result is found, match against the REQUEST BODY.
3. If more than one result is found, match against the REQUEST HEADERS.
4. If more than one result is found at this point, return an error.

## Error Responses
The following error responses may be returned from the container. They will always throw a X99 code in order not to conflict with any 4XX or 5XX responses you wish to legimately test for in your code. Server errors are logged and are most often the result of malformed JSON in the mock file.

### Route Not Matched
In the event the server cannot match a request against any mocks it has loaded you will get the following response:
- StatusCode: 499
- Message: Route not matched in mocks loaded.

### Server Error
In the event the server throws an error, you will get the following response:
- StatusCode: 599
- Message: Route not matched, an error occurred.

## License
 
The MIT License (MIT)

Copyright (c) 2021 Brandon Atkinson

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
