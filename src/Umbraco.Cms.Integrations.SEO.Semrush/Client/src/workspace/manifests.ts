export const SEMRUSH_WORKSPACE_ALIAS = "Umb.Workspace.Semrush";

const workspaceView: Array<UmbExtensionManifest> = [
    {
        type: "workspaceView",
        alias: "Umb.WorkspaceView.Semrush.View",
        name: "Umbraco Integration Workspace for Semrush",
        element: () => import("./semrush-workspace.element"),
        weight: 30,
        meta: {
          label: "Semrush",
          pathname: "semrush",
          icon: "icon-files",
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