import type { ReactNode } from "react";

type ConfirmDialogProps = {
  title: string;
  message: ReactNode;
  confirmLabel: string;
  cancelLabel: string;
  isBusy: boolean;
  onConfirm: () => void;
  onCancel: () => void;
};

/**
 * A modal confirmation. The dialog role, the initial focus on the safe action, and Escape to cancel
 * keep it operable by keyboard without a focus-trap dependency.
 */
export function ConfirmDialog({
  title,
  message,
  confirmLabel,
  cancelLabel,
  isBusy,
  onConfirm,
  onCancel,
}: ConfirmDialogProps) {
  return (
    <div className="confirm-dialog-backdrop">
      <div
        role="dialog"
        aria-modal="true"
        aria-labelledby="confirm-dialog-title"
        className="confirm-dialog"
        onKeyDown={(event) => {
          if (event.key === "Escape" && !isBusy) {
            event.stopPropagation();
            onCancel();
          }
        }}
      >
        <h2 id="confirm-dialog-title">{title}</h2>
        <p>{message}</p>
        <div className="form-actions">
          <button type="button" onClick={onConfirm} disabled={isBusy}>
            {confirmLabel}
          </button>
          <button type="button" onClick={onCancel} disabled={isBusy} autoFocus>
            {cancelLabel}
          </button>
        </div>
      </div>
    </div>
  );
}