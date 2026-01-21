export const GOOGLESEARCHCONSOLE_WORKSPACE_ALIAS = "Umb.Workspace.GoogleSearchConsole";

const workspaceView: Array<UmbExtensionManifest> = [
    {
        type: "workspaceView",
        alias: "Umb.WorkspaceView.GoogleSearchConsole.View",
        name: "Umbraco Integration Workspace for GoogleSearchConsole - URL Inspection Tool",
        element: () => import("./googlesearchconsole-workspace.element"),
        weight: 30,
        meta: {
          label: "URL Inspection",
          pathname: "urlInspectionTool",
          icon: "icon-search",
        },
        conditions: [
          {
            alias: "Umb.Condition.WorkspaceAlias",
            match: "Umb.Workspace.Document",
          },
        ],
      },
];

export const manifests: Array<UmbExtensionManifest> = [...workspaceView];