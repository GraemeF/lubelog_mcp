# MCP Server for LubeLogger

<img width="735" height="306" alt="image" src="https://github.com/user-attachments/assets/85887c27-48aa-42ff-b9db-f5a3fe225a9a" />

This is a technical tool mainly for experimentation and exploration into AI-enabled features in LubeLogger. 

AI and MCP are still evolving technologies, implementations of this project is subject to break without prior notice.

## Pre-requisites
- AI agent with ability to call tools/external integrations(i.e.: Claude Desktop)
- Node.js(for npx)

Sample Claude Config:

```
{
  "mcpServers": {
    "lubelogger": {
      "command": "npx",
      "args": [
        "mcp-remote",
        "http://localhost:5105/api/mcp"
      ]
    }
  }
}
```

## Example Usage

<img width="760" height="700" alt="image" src="https://github.com/user-attachments/assets/a99f4570-adae-406c-914c-d580fcf3cce6" />
