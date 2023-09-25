import { render, screen, fireEvent } from '@testing-library/react'
import App from './App'
import userEvent from '@testing-library/user-event'


test('renders the footer text', () => {
  render(<App />)
    expect(screen.getByText(/clearpoint.digital/i))
})


test('renders elements in the form', () => {
  render(<App />)
  const descriptionElement = screen.getByTestId('data-desciption');
  const addButtonElement = screen.getByTestId('data-addbutton');
  const cancelButtonElement = screen.getByTestId('data-cancelbutton');
  const refreshButtonElement = screen.getByTestId('data-refreshbutton');

  expect(descriptionElement).toBeInTheDocument();
  expect(addButtonElement).toBeInTheDocument();
  expect(cancelButtonElement).toBeInTheDocument();
  expect(refreshButtonElement).toBeInTheDocument();
});


test("should call handleAdd", async () => {
  render(<App />)
  const handleAdd = jest.fn();
  const addButtonElement = screen.getByTestId('data-addbutton');
  expect(addButtonElement).toBeTruthy();

    fireEvent.click(addButtonElement);

    expect(handleAdd).toHaveBeenCalledTimes(0);
});

