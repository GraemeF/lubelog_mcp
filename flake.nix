{
  description = "LubeLogger MCP Server";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
  };

  outputs =
    { self, nixpkgs }:
    let
      supportedSystems = [
        "x86_64-linux"
        "aarch64-linux"
      ];
      forAllSystems = nixpkgs.lib.genAttrs supportedSystems;
    in
    {
      packages = forAllSystems (
        system:
        let
          pkgs = nixpkgs.legacyPackages.${system};
        in
        {
          default = pkgs.buildDotnetModule {
            pname = "lubelog-mcp";
            version = "0.1.1";
            src = ./.;

            projectFile = "LubeLogMCP/LubeLogMCP.csproj";
            nugetDeps = ./deps.json;

            dotnet-sdk = pkgs.dotnetCorePackages.sdk_10_0;
            dotnet-runtime = pkgs.dotnetCorePackages.aspnetcore_10_0;

            executables = [ "LubeLogMCP" ];
          };

          container = pkgs.dockerTools.buildLayeredImage {
            name = "lubelog-mcp";
            tag = "latest";
            contents = [ self.packages.${system}.default ];
            fakeRootCommands = ''
              mkdir -p tmp
            '';
            config = {
              Cmd = [ "${self.packages.${system}.default}/bin/LubeLogMCP" ];
              ExposedPorts."8080/tcp" = { };
              Env = [
                "ASPNETCORE_URLS=http://+:8080"
                "TMPDIR=/tmp"
              ];
            };
          };
        }
      );

      devShells = forAllSystems (
        system:
        let
          pkgs = nixpkgs.legacyPackages.${system};
        in
        {
          default = pkgs.mkShell {
            packages = [ pkgs.dotnetCorePackages.sdk_10_0 ];
          };
        }
      );
    };
}
