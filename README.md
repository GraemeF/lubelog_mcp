# MCP Server for LubeLogger

<img width="735" height="306" alt="image" src="https://github.com/user-attachments/assets/85887c27-48aa-42ff-b9db-f5a3fe225a9a" />

This is a technical tool mainly for experimentation and exploration into AI-enabled features in LubeLogger. 

AI and MCP are still evolving technologies, implementations of this project is subject to break without prior notice.

## Pre-requisites
- AI agent with ability to call tools/external integrations(i.e.: Claude Desktop)
- Node.js(for npx)

Sample Claude Config for local or no auth:

```
{
  "mcpServers": {
    "lubelogger": {
      "command": "npx",
      "args": [
        "mcp-remote",
        "http://localhost:40150/api/mcp"
      ]
    }
  }
}
```

Sample Claude Config for header-based auth:

```
{
  "mcpServers": {
    "lubelogger": {
      "command": "npx",
      "args": [
        "mcp-remote",
        "http://localhost:5105/api/mcp",
	"--header",
        "Authorization:${AUTH_HEADER}"
      ],
      "env": {
        "AUTH_HEADER": "Basic <auth-token>" 
      }
    }
  }
}
```

`<auth-token>` is a base64 encoded string of your credentials in the following format `username:password` 

Example, username test and password 1234 will result in `dGVzdDoxMjM0`

## Configuration

Inject the following environment variables

- `LUBELOG_INSTANCE` - where the lubelogger instance is hosted(required)
- `LUBELOG_USER` - username to the lubelogger instance(required if auth is configured, skip if using header auth)
- `LUBELOG_PASS` - password to the lubelogger instance(required if auth is configured, skip if using header auth)

## Getting Started

1. Clone `docker-compose.yml` file from this repo
2. Open `docker-compose.yml` and inject the environment variables above
3. Save `docker-compose.yml`
4. Run `docker compose up -d`

## Commands Supported

- Retrieve list of vehicles
- Add Fuel Record from image of receipt
- Add Service/Repair/Upgrade records from invoice
- Add Odometer record from image of dashboard
- Add Supply(or shop supply) record from image of invoice
- Get latest odometer reading from vehicle
- Check status of LubeLogger instance

## Example Usage

<img width="760" height="700" alt="image" src="https://github.com/user-attachments/assets/a99f4570-adae-406c-914c-d580fcf3cce6" />

<img width="758" height="566" alt="image" src="https://github.com/user-attachments/assets/abf9ef37-7cfe-4701-b1b7-272e8a474841" />

<img width="735" height="561" alt="image" src="https://github.com/user-attachments/assets/a032512f-66c4-4a0a-8f98-54f7d28a623a" />
