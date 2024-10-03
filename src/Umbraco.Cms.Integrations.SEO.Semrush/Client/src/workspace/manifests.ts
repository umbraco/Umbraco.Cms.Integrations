import type { ManifestWorkspaceView, ManifestTypes, ManifestWorkspace } from '@umbraco-cms/backoffice/extension-registry';

export const SEMRUSH_WORKSPACE_ALIAS = "Umb.Workspace.Semrush";

const workspace: ManifestWorkspace = {
    type: "workspace",
    kind: "routable",
    alias: SEMRUSH_WORKSPACE_ALIAS,
    name: "Form Workspace",
    api: () => import("./semrush-workspace.context"),
    meta: {
      entityType: 'block-grid-type',
    },
  };

const workspaceView: Array<ManifestWorkspaceView> = [
    {
        type: "workspaceView",
        alias: "Umb.WorkspaceView.Semrush.View",
        name: "Umbraco Integration Workspace for Semrush",
        element: () => import("./semrush-workspace.element"),
        weight: 30,
        meta: {
          label: "Semrush",
          pathname: "semrush",
          icon: "info",
        },
        conditions: [
          {
            alias: "Umb.Condition.WorkspaceAlias",
            match: SEMRUSH_WORKSPACE_ALIAS,
          },
        ],
      },
];

export const manifests: Array<ManifestTypes> = [...workspaceView, workspace];