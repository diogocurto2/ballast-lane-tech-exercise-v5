import { describe, expect, it, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { ConfirmDialog } from "../../src/components/ConfirmDialog";

function renderDialog(overrides: Partial<Parameters<typeof ConfirmDialog>[0]> = {}) {
  const onConfirm = vi.fn();
  const onCancel = vi.fn();

  render(
    <ConfirmDialog
      title="Delete this task?"
      message="It will be permanently removed."
      confirmLabel="Yes, delete"
      cancelLabel="Cancel"
      isBusy={false}
      onConfirm={onConfirm}
      onCancel={onCancel}
      {...overrides}
    />,
  );

  return { onConfirm, onCancel };
}

describe("ConfirmDialog keyboard operation", () => {
  it("starts with focus on the cancel action so a stray Enter cannot confirm", () => {
    renderDialog();

    expect(screen.getByRole("button", { name: /cancel/i })).toHaveFocus();
  });

  it("cancels on Escape without confirming", async () => {
    const { onConfirm, onCancel } = renderDialog();

    await userEvent.keyboard("{Escape}");

    expect(onCancel).toHaveBeenCalledTimes(1);
    expect(onConfirm).not.toHaveBeenCalled();
  });

  it("ignores Escape while the action is in flight", async () => {
    const { onCancel } = renderDialog({ isBusy: true });

    await userEvent.keyboard("{Escape}");

    expect(onCancel).not.toHaveBeenCalled();
  });
});
